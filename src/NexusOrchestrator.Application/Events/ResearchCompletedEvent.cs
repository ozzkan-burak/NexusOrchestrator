using MediatR;
using NexusOrchestrator.Core.Events;
using System;

namespace NexusOrchestrator.Application.Events;

// INotification arayuzu, MediatR kutuphanesine bunun bir "Olay" (Event) oldugunu soyler.
// IDomainEvent arayuzu ise Core katmanindaki standartlarimiza uydugumuzu gosterir.
public class ResearchCompletedEvent : INotification, IDomainEvent
{
  // Olayin benzersiz kimligini tutariz.
  public Guid EventId { get; }

  // Olayin firlatildigi ani kaydederiz.
  public DateTime OccurredOn { get; }

  // Arastirmaci ajanin isini bitirdikten sonra uretecegi arastirma metni.
  public string ResearchContent { get; }

  // Olay nesnesi olusturulurken bu degerleri baslangic durumu (state) olarak atiyoruz.
  public ResearchCompletedEvent(string researchContent)
  {
    // Benzersiz bir kimlik (GUID) atamasi yapiyoruz.
    EventId = Guid.NewGuid();
    // Su anki zamani UTC olarak kaydediyoruz ki farkli sunucu saatleri sorun yaratmasin.
    OccurredOn = DateTime.UtcNow;
    // Ajanin urettigi icerigi olayin icine (Payload) yerlestiriyoruz.
    ResearchContent = researchContent;
  }
}
