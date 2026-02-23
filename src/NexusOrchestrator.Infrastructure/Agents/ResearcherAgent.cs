// src/NexusOrchestrator.Infrastructure/Agents/ResearcherAgent.cs

using Microsoft.SemanticKernel;
using NexusOrchestrator.Core.Agents;
using NexusOrchestrator.Infrastructure.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Infrastructure.Agents;

// IAgent arayuzunu uygulayan arastirmaci ajan sinifimiz.
public class ResearcherAgent : IAgent
{
  // Ajanin sistemdeki tekil adi.
  public string Name => "ResearcherAgent";

  // Ajanin amacini belirten aciklama metni.
  public string Description => "Verilen konuyu arastiran ve sonuclari derleyen ajan.";

  // Karar verici mekanizma olan Kernel nesnesi.
  private readonly Kernel _kernel;

  // Wikipedia aramasi yapacak eklenti sinifinin dogrudan ornegi.
  private readonly WebSearchPlugin _webSearchPlugin;

  // Constructor uzerinden Kernel'i DI konteynerinden aliyoruz.
  public ResearcherAgent(Kernel kernel)
  {
    // Kernel nesnesini sinif seviyesindeki degiskene atiyoruz.
    _kernel = kernel;

    // Eklentiyi LLM'in otonomisine birakmak yerine dogrudan sinifin icinde baslatiyoruz.
    _webSearchPlugin = new WebSearchPlugin();
  }

  // Ajanin gorevini baslatan ana metot.
  public async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken = default)
  {
    // 1. ADIM: Modeli aradan cikarip gercek zamanli aramayi C# uzerinden biz yapiyoruz.
    // Bu sayede kucuk modellerin halusinasyon gorme riskini tamamen sifirliyoruz.
    string searchResult = await _webSearchPlugin.SearchWikipediaAsync(input);

    // 2. ADIM: LLM'e gorevini ve buldugumuz C# verisini anlatan sistem komutu.
    string systemPrompt = @"Analyze the following raw data. Extract the key information and rewrite it into a clear paragraph. Do not add any extra explanations. Raw Data: {{$searchData}}";
    Console.WriteLine(searchResult);
    // Kernel uzerinden sistem komutunu (prompt) ve parametreleri calistiriyoruz.
    // Artik ToolCallBehavior.AutoInvokeKernelFunctions ayarina ihtiyacimiz yok.
    var result = await _kernel.InvokePromptAsync(
        systemPrompt,
        new KernelArguments
        {
                // Prompt icindeki yer tutuculara disaridan aldigimiz guvenilir verileri aktariyoruz.
                { "input", input },
                { "searchData", searchResult }
        });

    // Üretilen nihai yaniti donduruyoruz.
    return result.GetValue<string>() ?? "Arastirma sirasinda bir sonuc uretilemedi.";
  }
}