using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;
using ApiRefactor.Tests.Support;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ApiRefactor.Api.Controllers;
using ApiRefactor.Application;

namespace ApiRefactor.Tests.Controllers;

[TestFixture]
/// <summary>
/// Tests for the WavesController class.
/// </summary>
public class WaveControllerTests
{
    private InMemorySqliteConnectionFactory _factory = default!;
    private SqliteWaveRepository _repo = default!;
    private WaveController _controller = default!;
    private IWaveService _service = default!;
    /// <summary>
    /// Initializes the test environment.
    /// </summary>
    [SetUp]
    public async Task SetUp()
    {
        // Use shared in-memory database across multiple connections
        _factory = new InMemorySqliteConnectionFactory("Data Source=waves_db;Mode=Memory;Cache=Shared");
        await SchemaHelper.EnsureSchemaAsync(_factory);
        _repo = new SqliteWaveRepository(_factory);
        _service = new WaveService(_repo); 
        _controller = new WaveController(_service);
    }
    /// <summary>
    /// Cleans up the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (_factory is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    /// <summary>
    /// Tests the GetAll method of the WavesController.
    /// </summary>
    [Test]
    public async Task GetAll_ReturnsOk_WithCollection()
    {
        await _repo.UpsertAsync(new Wave { Name = "CtrlA", WaveDate = DateTime.UtcNow });
        await _repo.UpsertAsync(new Wave { Name = "CtrlB", WaveDate = DateTime.UtcNow });

        var actionResult = await _controller.GetAll(CancellationToken.None);
        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)actionResult.Result!;
        var wavesEnvelope = (Waves)ok.Value!;
        Assert.That(wavesEnvelope.Items.Count, Is.EqualTo(2));
    }
}