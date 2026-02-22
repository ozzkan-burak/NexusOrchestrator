
namespace NexusOrchestrator.Core.Agents;

// Sistemdeki tum otonom ajanlarin (Researcher, Summarizer vb.) uymasi gereken sablon.
public interface IAgent
{
  // Ajanin sistem icindeki benzersiz adi (Ornegin: "ResearchAgent").
  string Name { get; }

  // Ajanin ne is yaptigini anlatan kisa aciklama.
  // Bu alan, Semantic Kernel tarafindan ajanin yeteneklerini anlamak icin kullanilacaktir.
  string Description { get; }

  // Ajanin ana gorevini tetikleyen asenkron metot.
  // input: Ajana verilen gorev veya baglam (context).
  // cancellationToken: Uzun suren islemleri iptal edebilmek icin standart .NET yapisi.
  Task<string> ExecuteAsync(string input, CancellationToken cancellationToken = default);
}