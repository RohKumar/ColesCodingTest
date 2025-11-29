
using ApiRefactor.Application;
using ApiRefactor.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ApiRefactor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WavesController : ControllerBase
{
    private readonly IWaveService _service;
    public WavesController(IWaveService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Wave>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Wave>> GetById(Guid id, CancellationToken ct)
    {
        var wave = await _service.GetByIdAsync(id, ct);
        return wave is null ? NotFound() : Ok(wave);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] Wave wave, CancellationToken ct)
    {
        await _service.UpsertAsync(wave, ct);
        return NoContent();
    }
}