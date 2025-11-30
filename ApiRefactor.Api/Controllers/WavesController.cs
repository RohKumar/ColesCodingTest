using ApiRefactor.Application;
using ApiRefactor.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ApiRefactor.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Controller for managing wave entities.
/// </summary>
public class WaveController : ControllerBase
{
    private readonly IWaveService _service;
    public WaveController(IWaveService service) => _service = service;
    /// <summary>
    /// Retrieves all wave entities.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Waves))]
    public async Task<ActionResult<Waves>> GetAll(CancellationToken ct)
    {
        var waves = await _service.GetAllAsync(ct);
        return Ok(new Waves { Items = waves.ToList() });
    }
    /// <summary>
    /// Retrieves a wave entity by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Wave>> GetById(Guid id, CancellationToken ct)
    {
        var wave = await _service.GetByIdAsync(id, ct);
        return wave is null ? NotFound() : Ok(wave);
    }
    /// <summary>
    /// Creates a new wave entity.
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] Wave wave, CancellationToken ct)
    {
        await _service.UpsertAsync(wave, ct);
        return NoContent();
    }

    // Uncomment to test exception handling middleware
    //public IActionResult Get()
    //{
    //    throw new InvalidOperationException("Boom");
    //}
}