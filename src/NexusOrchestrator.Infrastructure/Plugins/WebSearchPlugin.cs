// src/NexusOrchestrator.Infrastructure/Plugins/WebSearchPlugin.cs

using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace NexusOrchestrator.Infrastructure.Plugins;

public class WebSearchPlugin
{
  private readonly HttpClient _httpClient;

  public WebSearchPlugin()
  {
    // Yeni bir HttpClient nesnesi ornegi olusturuyoruz.
    _httpClient = new HttpClient();

    // Wikipedia API'si anonim istekleri 403 Forbidden ile reddeder.
    // Bu yuzden kendimizi tanitan bir User-Agent basligi (header) ekliyoruz.
    _httpClient.DefaultRequestHeaders.Add("User-Agent", "NexusOrchestrator/1.0 (Local Development)");
  }

  [KernelFunction("SearchWikipedia")]
  [Description("Belirli bir konu hakkinda Wikipedia uzerinden gercek zamanli ansiklopedik arama yapar ve kisa bilgi doner.")]
  public async Task<string> SearchWikipediaAsync(
      [Description("Aranacak kavram, kisi veya nesne")] string query)
  {
    // Gelen metindeki bosluklari ve ozel karakterleri URL formatina (%20 vb.) ceviriyoruz.
    string encodedQuery = Uri.EscapeDataString(query);

    // URL'i guvenli hale getirilmis parametre ile olusturuyoruz.
    string url = $"https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch={encodedQuery}&utf8=&format=json";

    // HttpClient ile Wikipedia API'sine GET istegi atiyoruz. 
    // User-Agent eklendigi icin artik 403 yetki hatasi almayacagiz.
    var response = await _httpClient.GetStringAsync(url);

    // Gelen JSON string yanitini isleyebilmek icin JsonDocument nesnesine donusturuyoruz.
    using JsonDocument document = JsonDocument.Parse(response);

    // JSON agaci icerisinden "query" dugumunun altindaki "search" dizisini aliyoruz.
    var searchResults = document.RootElement.GetProperty("query").GetProperty("search");

    // Eger arama sonucu bossa modele otonomiyi kaybetmemesi icin anlamli bir metin donuyoruz.
    if (searchResults.GetArrayLength() == 0)
    {
      return "Arama sonucu bulunamadi. Lutfen baska bir kelime deneyin.";
    }

    // Dizideki ilk sonucun "snippet" (kisa ozet) ozelligini aliyoruz.
    string snippet = searchResults[0].GetProperty("snippet").GetString() ?? "";

    // Wikipedia'nin dondugu HTML vurgu etiketlerini temizleyerek LLM icin saf metni donuyoruz.
    return snippet.Replace("<span class=\"searchmatch\">", "").Replace("</span>", "");
  }
}