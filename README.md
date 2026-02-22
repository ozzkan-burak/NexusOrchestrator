# NexusOrchestrator

NexusOrchestrator, birbirinden izole ve otonom çalışan yapay zeka ajanlarının (AI Agents) görevlerini merkezi bir olay veriyolu (Event Bus) üzerinden koordine eden, .NET tabanlı bir orkestrasyon motorudur.

Bu proje, geleneksel "spagetti" API çağrıları yerine, gevşek bağlı (Decoupled) bir mimari kullanarak ajanların birbirini tanımadan ortak bir hedefe ulaşmasını sağlar.

## Architecture & Design Patterns

Sistem, sürdürülebilirliği ve ölçeklenebilirliği garanti altına almak için aşağıdaki mimari desenler üzerine inşa edilmiştir:

1.  **Clean Architecture:** Çekirdek iş mantığının (Domain), dış dünyadan (Infrastructure/Presentation) tamamen izole edilmesi.
2.  **Event-Driven Architecture (EDA):** Ajanlar arası iletişimin doğrudan (Request/Response) değil, olaylar (Events) aracılığıyla asenkron olarak sağlanması.
3.  **Mediator Pattern (MediatR):** Sistem içi mesajlaşmanın tek bir merkezden yönetilerek bağımlılıkların (Coupling) en aza indirilmesi.
4.  **Semantic Kernel Integration:** Microsoft'un kurumsal AI standardı kullanılarak, ajanlara C# fonksiyonlarının "Yetenek" (Plugin/Tool) olarak tanımlanması.

## System Flow (Mermaid Diagram)

Aşağıdaki şema, kullanıcıdan gelen bir isteğin sistem içinde nasıl olaylara (Events) dönüştüğünü ve ajanlar arasında nasıl dolaştığını göstermektedir.

```mermaid
graph TD
  %% İstemci ve API Katmanı
  A[Client Request] -- "1. Start Orchestration" --> B[Presentation API]

  subgraph Message_Bus [MediatR - In-Memory Event Bus]
      %% Merkezi mesajlaşma omurgası
      C{Mediator Pipeline}
  end

  subgraph Application_Layer [Application Layer - AI Agents]
      %% Semantic Kernel ile güçlendirilmiş otonom ajanlar
      D[Researcher Agent]
      E[Summarizer Agent]
      F[Markdown Writer Agent]
  end

  %% Olay Güdümlü Akış (Event-Driven Flow)
  B -- "2. Publish(AnalyzeTopicCommand)" --> C

  C -- "3. Route Command" --> D
  D -- "4. Execute SK Plugins (Web Search)" --> D
  D -- "5. Publish(ResearchCompletedEvent)" --> C

  C -- "6. Route Event" --> E
  E -- "7. Publish(SummaryCompletedEvent)" --> C

  C -- "8. Route Event" --> F
  F -- "9. Publish(DocumentCreatedEvent)" --> C

  C -- "10. Return Final Result" --> B
```
