using MediatR;
using NexusOrchestrator.Application.Events;
using NexusOrchestrator.Core.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Application.Handlers;

// ResearchCompletedEvent olayini dinleyen sinif.
public class ResearchCompletedEventHandler : INotificationHandler<ResearchCompletedEvent>
{
  // Sistemdeki ajanlari tutacagimiz degisken.
  private readonly IEnumerable<IAgent> _agents;

  // Yeni olaylari firlatmak icin kullanacagimiz yayinci nesne.
  private readonly IPublisher _publisher;

  // Hem ajanlari hem de yayinci nesneyi Dependency Injection (DI) uzerinden aliyoruz.
  public ResearchCompletedEventHandler(IEnumerable<IAgent> agents, IPublisher publisher)
  {
    // Gelen ajanlari sinif seviyesindeki degiskene atiyoruz.
    _agents = agents;

    // Gelen yayinci nesneyi sinif seviyesindeki degiskene atiyoruz.
    _publisher = publisher;
  }

  // Olay yakalandiginda MediatR tarafindan calistirilan ana metot.
  public async Task Handle(ResearchCompletedEvent notification, CancellationToken cancellationToken)
  {
    // Ajanlar listesinden adi "SummarizerAgent" olani ariyoruz.
    var summarizerAgent = _agents.FirstOrDefault(a => a.Name == "SummarizerAgent");

    // Ajan bulunamazsa isleme devam edemeyecegimiz icin hata firlatiyoruz.
    if (summarizerAgent == null)
    {
      // Hata mesajini loglanabilir sekilde firlatiyoruz.
      throw new InvalidOperationException("SummarizerAgent sistemde bulunamadi.");
    }

    // Bulunan ozetleyici ajani, arastirma metniyle (notification.ResearchContent) tetikliyoruz.
    string summaryResult = await summarizerAgent.ExecuteAsync(notification.ResearchContent, cancellationToken);

    // Ozetleme islemi bitti, yeni firlatilacak olayi hazirliyoruz.
    var summaryCompletedEvent = new SummaryCompletedEvent(summaryResult);

    // Olusturdugumuz yeni olayi MediatR uzerinden tum sisteme yayinliyoruz.
    await _publisher.Publish(summaryCompletedEvent, cancellationToken);
  }
}