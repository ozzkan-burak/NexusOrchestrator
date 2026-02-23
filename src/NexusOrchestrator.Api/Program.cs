// src/NexusOrchestrator.Api/Program.cs

using Microsoft.SemanticKernel;
using NexusOrchestrator.Application.Commands;
using NexusOrchestrator.Core.Agents;
using NexusOrchestrator.Infrastructure.Agents;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONTROLLER VE SWAGGER KAYITLARI ---
// Web API'nin temel iskeletini ve dokumantasyon arayuzunu sisteme ekler.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. MEDIATR KAYDI ---
// StartResearchCommand sinifinin bulundugu derlemeyi tarayarak tum komut ve olay dinleyicilerini bulur.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(StartResearchCommand).Assembly));

// --- 3. SEMANTIC KERNEL VE OLLAMA (LOCAL LLM) KAYDI ---
// Yerel makinede calisan Ollama servisini, Semantic Kernel'in OpenAI bileseni uzerinden sisteme bagliyoruz.
builder.Services.AddKernel()
    .AddOpenAIChatCompletion(
        modelId: "llama3.2:1b", // Sadece 1.3 GB boyutundaki islemci (CPU) dostu hafif modelimiz.
        apiKey: "LOCAL_MOCK_KEY", // Ollama anahtar dogrulamasi yapmaz, ancak metodun imzasini karsilamak icin sembolik yazilir.
        endpoint: new Uri("http://localhost:11434/v1") // Ollama'nin yerel API uc noktasi.
    );

// --- 4. AJANLARIN (AGENTS) KAYDI ---
// Ajanlarin birbirinden izole (state-free) calismasi icin her cagirilisinda yeni bir ornek (Transient) uretilir.
builder.Services.AddTransient<IAgent, ResearcherAgent>();
builder.Services.AddTransient<IAgent, SummarizerAgent>();
builder.Services.AddTransient<IAgent, MarkdownWriterAgent>();

var app = builder.Build();

// --- HTTP ISTEK HATTI (PIPELINE) AYARLARI ---
// Sadece gelistirme ortaminda (Development) arayuzun erisilebilir olmasini saglariz.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Guvenlik ve yonlendirme katmanlari.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Sunucuyu ayaga kaldirir.
app.Run();