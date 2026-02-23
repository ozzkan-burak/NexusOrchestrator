# NexusOrchestrator

NexusOrchestrator, birbirinden izole ve otonom çalışan yapay zeka ajanlarının (AI Agents) görevlerini merkezi bir olay veriyolu (Event Bus) üzerinden koordine eden, .NET tabanlı bir orkestrasyon motorudur.

Bu proje, geleneksel "spagetti" API çağrıları yerine, gevşek bağlı (Decoupled) bir mimari kullanarak ajanların birbirini tanımadan ortak bir hedefe ulaşmasını sağlar.

## Architecture & Design Patterns

Sistem, sürdürülebilirliği ve ölçeklenebilirliği garanti altına almak için aşağıdaki mimari desenler üzerine inşa edilmiştir:

1.  **Clean Architecture:** Çekirdek iş mantığının (Domain), dış dünyadan (Infrastructure/Presentation) tamamen izole edilmesi.
2.  **Event-Driven Architecture (EDA):** Ajanlar arası iletişimin doğrudan (Request/Response) değil, olaylar (Events) aracılığıyla asenkron olarak sağlanması.
3.  **Mediator Pattern (MediatR):** Sistem içi mesajlaşmanın tek bir merkezden yönetilerek bağımlılıkların (Coupling) en aza indirilmesi.
4.  **Semantic Kernel Integration:** Microsoft'un kurumsal AI standardı kullanılarak, ajanlara C# fonksiyonlarının "Yetenek" (Plugin/Tool) olarak tanımlanması.
5.  **Local-First AI:** Dışa bağımlılığı ve maliyeti sıfırlamak için Ollama üzerinden yerel (Local) SLM (Small Language Model) entegrasyonu.

## System Flow

Aşağıdaki şema, kullanıcıdan gelen bir isteğin sistem içinde nasıl olaylara (Events) dönüştüğünü ve ajanlar arasında nasıl dolaştığını göstermektedir.

```mermaid
graph TD
  A[Client Request] -- "1. Start Orchestration" --> B[Presentation API]

  subgraph Message_Bus [MediatR - In-Memory Event Bus]
      C{Mediator Pipeline}
  end

  subgraph Application_Layer [Application Layer - AI Agents]
      D[Researcher Agent]
      E[Summarizer Agent]
      F[Markdown Writer Agent]
  end

  B -- "2. Publish(StartResearchCommand)" --> C
  C -- "3. Route Command" --> D
  D -- "4. Execute SK Plugins (Wikipedia Search)" --> D
  D -- "5. Publish(ResearchCompletedEvent)" --> C
  C -- "6. Route Event" --> E
  E -- "7. Publish(SummaryCompletedEvent)" --> C
  C -- "8. Route Event" --> F
  F -- "9. Publish(DocumentCreatedEvent)" --> C
  C -- "10. Return Final Result" --> B
```

## Folder Structure

Proje dizin yapısı, Bağımlılık Kuralı'na (Dependency Rule) sıkı sıkıya bağlı kalınarak oluşturulmuştur.

NexusOrchestrator/
│
├── src/
│ ├── NexusOrchestrator.Core/ # (Domain Layer) Arayüzler ve olay sözleşmeleri.
│ ├── NexusOrchestrator.Application/ # MediatR Handler'ları ve Event tanımları.
│ ├── NexusOrchestrator.Infrastructure/ # Semantic Kernel, Ajan implementasyonları, Plugin'ler.
│ └── NexusOrchestrator.Api/ # (Presentation Layer) REST API ve DI Container.
│
├── setup.sh # Proje iskeletini otomatize eden DX betiği.
├── docker-compose.yml # Altyapı servisleri (Seq, Redis).
└── README.md # Dokümantasyon.

## Getting Started (Nasıl Çalıştırılır)

Projeyi kendi ortamınızda çalıştırmak için aşağıdaki adımları sırasıyla izleyebilirsiniz.

Gereksinimler
.NET 8 SDK

Docker Desktop (Loglama altyapısı için)

Ollama (Yerel yapay zeka modeli için)

Adım 1: Proje Altyapısını Kurma
Eğer projeyi sıfırdan derlemek isterseniz, geliştirici deneyimini (DX) hızlandırmak için hazırlanan betiği kullanabilirsiniz. Kök dizinde terminali açın ve çalıştırın:

Bash
bash setup.sh
Adım 2: Yerel Yapay Zeka Modelini Başlatma
Sistem, maliyetleri sıfırlamak adına lokal bir model ile çalışacak şekilde yapılandırılmıştır. Ollama'yı kurduktan sonra terminalde hafif ve hızlı olan modeli ayağa kaldırın:

Bash
ollama run llama3.2:1b
(Bu terminal penceresini arka planda açık bırakın).

Adım 3: İzlenebilirlik (Observability) Servislerini Başlatma
Sistemdeki olay (Event) akışlarını ve ajanların arka planda aldığı kararları net bir şekilde izleyebilmek için Seq loglama sunucusunu ayağa kaldırın:

Bash
docker-compose up -d
(Seq arayüzüne http://localhost:5341 adresinden erişebilirsiniz).

Adım 4: API'yi Çalıştırma ve Test Etme
API dizinine giderek projeyi başlatın:

Bash
cd src/NexusOrchestrator.Api
dotnet run
Tarayıcınızda http://localhost:<port>/swagger adresine gidin. POST /api/orchestration/start metodunu kullanarak aşağıdaki örnek JSON ile otonom süreci başlatabilirsiniz:

## Örnek Sorgu JSON

```json
{
  "topic": "Nikola Tesla'nin alternatif akim calismalari"
}
```
