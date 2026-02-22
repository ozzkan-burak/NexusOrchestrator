
namespace NexusOrchestrator.Core.Events;

// Sistemdeki tüm domain olaylarinin (event) türeyeceği temel arayuz (interface).
// Bu arayuz, olaylarin MediatR veya baska bir kutuphaneye bagimli olmasini engeller.
public interface IDomainEvent
{
  // Olayin benzersiz kimligi. Loglama ve takip (trace) icin kullanilir.
  Guid EventId { get; }

  // Olayin gerceklestigi zaman damgasi. Olaylarin sirasini (nedensellik) belirlemek icin kritiktir.
  DateTime OccurredOn { get; }
}