
using MediatR;

namespace NexusOrchestrator.Application.Commands;

// IRequest<string>: MediatR kütüphanesine, bu komutun işlendiğinde
// geriye 'string' tipinde bir sonuc (örneğin araştırma özeti) döneceğini belirtir.
public class StartResearchCommand : IRequest<string>
{
  // Arastirilmasi istenen ana konu (Prompt).
  public string Topic { get; set; }

  // Constructor ile konuyu zorunlu hale getiriyoruz.
  public StartResearchCommand(string topic)
  {
    Topic = topic;
  }
}