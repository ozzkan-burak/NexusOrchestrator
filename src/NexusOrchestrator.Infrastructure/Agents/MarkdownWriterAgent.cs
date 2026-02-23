using Microsoft.SemanticKernel;
using NexusOrchestrator.Core.Agents;
using System.Threading;
using System.Threading.Tasks;

namespace NexusOrchestrator.Infrastructure.Agents;

// IAgent arayuzunu uygulayan, zincirin son halkasi olan ajan sinifi.
public class MarkdownWriterAgent : IAgent
{
  // Ajanin sistemdeki benzersiz adi.
  public string Name => "MarkdownWriterAgent";

  // Ajanin gorev aciklamasi.
  public string Description => "Gelen metinleri kurumsal Markdown dokumani formatina donusturen ajan.";

  // Semantic Kernel nesnesi.
  private readonly Kernel _kernel;

  // Constructor uzerinden kernel nesnesini DI ile aliyoruz.
  public MarkdownWriterAgent(Kernel kernel)
  {
    // Kernel nesnesini sinif seviyesindeki degiskene atiyoruz.
    _kernel = kernel;
  }

  // Ajanin ana calistirma metodu.
  public async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken = default)
  {
    // LLM'e bu ajanin nasil davranmasi gerektigini soyleyen sistem komutu.
    string systemPrompt = @"Format the following text as a professional Markdown document with headings and bold text. Output only the Markdown content. Text: {{$input}}";

    // Kernel'i kullanarak promptu calistiriyoruz.
    var result = await _kernel.InvokePromptAsync(
        systemPrompt,
        new KernelArguments
        {
                // Prompt icindeki {{$input}} kismina ozetleyici ajandan gelen veriyi koyuyoruz.
                { "input", input }
        });

    // Ciktinin string halini aliyoruz.
    string finalMarkdown = result.GetValue<string>() ?? "Dokuman olusturulamadi.";

    // Gelecek adim: Burada uretilen Markdown metnini fiziksel bir .md dosyasi olarak
    // diske kaydetmek icin sistem IO islemleri veya Semantic Kernel pluginleri eklenebilir.

    // Olusturulan metni geri donduruyoruz.
    return finalMarkdown;
  }
}