using Microsoft.SemanticKernel;
using NexusOrchestrator.Core.Agents;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Infrastructure.Agents;

// Core katmanindaki IAgent sozlesmesini uygulayan ikinci ajan sinifimiz.
public class SummarizerAgent : IAgent
{
  // Ajanin sistemdeki benzersiz adi. Handler uzerinden bu isimle cagiracagiz.
  public string Name => "SummarizerAgent";

  // Ajanin gorev tanimi. Sistemdeki rolunu belirtir.
  public string Description => "Verilen uzun ve detayli metinleri yonetici ozeti formatina donusturen ajan.";

  // Semantic Kernel'in ana yonetici nesnesi (Beyin).
  private readonly Kernel _kernel;

  // Constructor uzerinden Kernel nesnesini Dependency Injection (DI) ile aliyoruz.
  public SummarizerAgent(Kernel kernel)
  {
    // Gelen kernel nesnesini sinif seviyesindeki degiskene atiyoruz.
    _kernel = kernel;
  }

  // IAgent arayuzunden gelen ana calistirma metodu.
  public async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken = default)
  {
    // Ajanin nasil davranmasi gerektigini belirten sistem komutu (System Prompt).
    // Arastirmacidan gelen uzun metin {{$input}} kismina dinamik olarak yerlesecek.
    string systemPrompt = @"Summarize the following text using bullet points. Output only the bulleted list. Text: {{$input}}";

    // Kernel nesnesini kullanarak promptu calistiriyoruz.
    // Ozetleme isleminde distan bir araca (web search vb.) ihtiyac olmadigi icin 
    // ToolCallBehavior ayarlarina bu asamada gerek duymuyoruz.
    var result = await _kernel.InvokePromptAsync(
        systemPrompt,
        new KernelArguments
        {
                // Prompt icindeki {{$input}} degiskenine parametreden gelen uzun arastirma metnini atiyoruz.
                { "input", input }
        });

    // Modelden donen nihai ozet metnini string formatinda donduruyoruz.
    // Eger null donerse varsayilan bir hata mesaji iletiyoruz.
    return result.GetValue<string>() ?? "Ozetleme islemi basarisiz oldu.";
  }
}