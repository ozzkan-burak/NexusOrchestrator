using MediatR;
using NexusOrchestrator.Application.Commands;
using NexusOrchestrator.Core.Agents;

namespace NexusOrchestrator.Application.Handlers;

// IRequestHandler<StartResearchCommand, string>: Bu sinifin 'StartResearchCommand' komutunu 
// isleyecegini ve geriye 'string' donecegini MediatR'a soyler.
public class StartResearchCommandHandler : IRequestHandler<StartResearchCommand, string>
{
  // İhtiyacimiz olan ajanlari Dependency Injection (DI) uzerinden alacagiz.
  // Şimdilik sistemdeki tüm ajanları bir liste olarak alıp, içinden Researcher olanı seçeceğiz.
  private readonly IEnumerable<IAgent> _agents;

  public StartResearchCommandHandler(IEnumerable<IAgent> agents)
  {
    _agents = agents;
  }

  // MediatR bu komutu yakaladiginda otomatik olarak Handle metodunu tetikler.
  public async Task<string> Handle(StartResearchCommand request, CancellationToken cancellationToken)
  {
    // 1. Sistemdeki ajanlar arasindan adi "ResearcherAgent" olani bul.
    var researcherAgent = _agents.FirstOrDefault(a => a.Name == "ResearcherAgent");

    // Eger ajan sisteme kayitli degilse, isleme devam edilemez, hata firlatilir.
    if (researcherAgent == null)
    {
      throw new InvalidOperationException("ResearcherAgent sistemde bulunamadi.");
    }

    // 2. Ajanin icindeki ana mantigi (Semantic Kernel) tetikle ve sonucu bekle.
    string result = await researcherAgent.ExecuteAsync(request.Topic, cancellationToken);

    // 3. (Gelecek Adim) Burada MediatR uzerinden 'ResearchCompletedEvent' firlatarak
    // diger ajanlari (Summarizer) haberdar edecegiz.

    // Sonucu geri don.
    return result;
  }
}