using MediatR;
using NexusOrchestrator.Application.Events;
using NexusOrchestrator.Core.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Application.Handlers;

// SummaryCompletedEvent olayini dinleyen abone sinif.
public class SummaryCompletedEventHandler : INotificationHandler<SummaryCompletedEvent>
{
  // Sistemdeki ajanlari bulmak icin kullanacagimiz degisken.
  private readonly IEnumerable<IAgent> _agents;

  // Dependency Injection uzerinden ajan listesini aliyoruz.
  public SummaryCompletedEventHandler(IEnumerable<IAgent> agents)
  {
    // Ajan listesini sinif icerisindeki degiskene atiyoruz.
    _agents = agents;
  }

  // Olay yakalandiginda calisacak ana metot.
  public async Task Handle(SummaryCompletedEvent notification, CancellationToken cancellationToken)
  {
    // Sistemdeki ajanlar arasindan Markdown yazdirmakla gorevli ajani ariyoruz.
    var markdownAgent = _agents.FirstOrDefault(a => a.Name == "MarkdownWriterAgent");

    // Ajan bulunamazsa hata firlatiyoruz.
    if (markdownAgent == null)
    {
      // Sistemin eksik yapilandirildigini bildiren hata firlatmasi.
      throw new InvalidOperationException("MarkdownWriterAgent sistemde bulunamadi.");
    }

    // Markdown ajanini, firlatilan olayin icindeki ozet metni ile calistiriyoruz.
    await markdownAgent.ExecuteAsync(notification.SummaryContent, cancellationToken);
  }
}