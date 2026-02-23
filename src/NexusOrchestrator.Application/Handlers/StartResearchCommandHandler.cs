
using MediatR;
using NexusOrchestrator.Application.Commands;
using NexusOrchestrator.Application.Events;
using NexusOrchestrator.Core.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Application.Handlers;

// IRequestHandler arayuzunu uygulayarak bu sinifin komut isleyici oldugunu belirtiyoruz.
public class StartResearchCommandHandler : IRequestHandler<StartResearchCommand, string>
{
  // Sistemdeki ajanlarin listesini tutacagimiz degisken.
  private readonly IEnumerable<IAgent> _agents;

  // Olaylari (event) firlatmak icin MediatR'in IPublisher arayuzunu tutacagimiz degisken.
  private readonly IPublisher _publisher;

  // Constructor uzerinden Dependency Injection (DI) ile ajanlari ve publisher nesnesini aliyoruz.
  public StartResearchCommandHandler(IEnumerable<IAgent> agents, IPublisher publisher)
  {
    // Gelen ajan listesini sinif seviyesindeki private degiskene atiyoruz.
    _agents = agents;
    // Gelen publisher nesnesini sinif seviyesindeki private degiskene atiyoruz.
    _publisher = publisher;
  }

  // MediatR tarafindan komut geldiginde otomatik olarak tetiklenecek olan ana isleyici metot.
  public async Task<string> Handle(StartResearchCommand request, CancellationToken cancellationToken)
  {
    // Sistemdeki ajanlar arasindan adi "ResearcherAgent" olani ariyoruz.
    var researcherAgent = _agents.FirstOrDefault(a => a.Name == "ResearcherAgent");

    // Eger arastirmaci ajan bulunamazsa sistemin calismasini durdurup hata firlatiyoruz.
    if (researcherAgent == null)
    {
      // Hatayi aciklayici bir mesajla firlatiyoruz, boylece loglardan kolayca izlenebilir.
      throw new InvalidOperationException("ResearcherAgent sistemde bulunamadi.");
    }

    // Ajanin icindeki ExecuteAsync metodunu cagirarak Semantic Kernel'i tetikliyor ve sonucu bekliyoruz.
    string result = await researcherAgent.ExecuteAsync(request.Topic, cancellationToken);

    // Ajan isini bitirdi, MediatR uzerinden firlatilacak olayi (event payload) olusturuyoruz.
    var researchCompletedEvent = new ResearchCompletedEvent(result);

    // Olusturdugumuz olayi MediatR uzerinden tum sisteme yayinliyoruz (Publish).
    await _publisher.Publish(researchCompletedEvent, cancellationToken);

    // Islem sonucunu baslangic noktasina (API katmanina) geri donduruyoruz.
    return result;
  }
}