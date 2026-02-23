using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexusOrchestrator.Application.Commands;

namespace NexusOrchestrator.Api.Controllers;

// [ApiController] niteligi, bu sinifin bir REST API kontrolcusu oldugunu belirtir 
// ve model dogrulama (validation) gibi islemleri otomatiklestirir.
[ApiController]
// [Route] niteligi, bu kontrolcuye erisim adresini (endpoint) belirler. 
// Ornegin: https://localhost:5001/api/orchestration
[Route("api/[controller]")]
public class OrchestrationController : ControllerBase
{
  // Istekleri sistemin merkezine (Application katmanina) iletecek olan MediatR nesnesi.
  private readonly IMediator _mediator;

  // Constructor uzerinden Dependency Injection (DI) ile IMediator arayuzunu aliyoruz.
  public OrchestrationController(IMediator mediator)
  {
    // Gelen mediator nesnesini sinif seviyesindeki degiskene atiyoruz.
    _mediator = mediator;
  }

  // [HttpPost] niteligi, bu metodun sadece HTTP POST isteklerine cevap verecegini belirtir.
  // "start" rotasi ile erisilir (api/orchestration/start).
  [HttpPost("start")]
  public async Task<IActionResult> StartOrchestration([FromBody] OrchestrationRequest request)
  {
    // Kullanicidan gelen konu (topic) bos ise 400 Bad Request donuyoruz.
    if (string.IsNullOrWhiteSpace(request.Topic))
    {
      return BadRequest("Arastirma konusu (Topic) bos birakilamaz.");
    }

    // 1. Api katmani, kullanicidan gelen istegi bir 'Command' (Komut) nesnesine donusturur.
    var command = new StartResearchCommand(request.Topic);

    // 2. MediatR, bu komutu alir ve Application katmanindaki StartResearchCommandHandler'a iletir.
    // Komut islenip ajanlar gorevini bitirdiginde sonuc (string) geri doner.
    var result = await _mediator.Send(command);

    // 3. Islemin basariyla tamamlandigini (200 OK) ve uretilen nihai sonucu istemciye (Client) donuyoruz.
    return Ok(new { Message = "Orkestrasyon basariyla tamamlandi.", Result = result });
  }
}

// Istemciden (Client) gelecek JSON verisinin modelini (Schema) tanimliyoruz.
public class OrchestrationRequest
{
  // Istemcinin gonderecegi 'Topic' alani. Ornegin: { "topic": "Kuantum Bilgisayarlar" }
  public string Topic { get; set; }
}