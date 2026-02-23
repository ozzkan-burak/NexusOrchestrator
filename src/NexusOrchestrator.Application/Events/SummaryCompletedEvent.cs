using MediatR;
using NexusOrchestrator.Core.Events;
using System;

namespace NexusOrchestrator.Application.Events;

// INotification ve IDomainEvent arayuzlerini uygulayarak bu sinifin sistemde firlatilacak bir olay oldugunu belirtiyoruz.
public class SummaryCompletedEvent : INotification, IDomainEvent
{
  // Olayin sistemdeki benzersiz kimligi (Trace ID).
  public Guid EventId { get; }

  // Olayin gerceklestigi zamani UTC formatinda tutan ozellik.
  public DateTime OccurredOn { get; }

  // Ozetleyici ajanin urettigi kisa yonetici ozeti metni.
  public string SummaryContent { get; }

  // Constructor uzerinden olay nesnesi uretilirken gerekli atamalari yapiyoruz.
  public SummaryCompletedEvent(string summaryContent)
  {
    // Yeni bir benzersiz kimlik olusturuyoruz.
    EventId = Guid.NewGuid();

    // Olayin olustugu anki zamani kaydediyoruz.
    OccurredOn = DateTime.UtcNow;

    // Parametre olarak gelen ozet metnini olayin icine (Payload) yerlestiriyoruz.
    SummaryContent = summaryContent;
  }
}