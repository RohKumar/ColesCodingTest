using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;
using ApiRefactor.Tests.Support;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ApiRefactor.Api.Controllers;
using ApiRefactor.Application;

namespace ApiRefactor.Tests.Controllers;

[TestFixture]
public class WavesControllerTests
{
    private InMemorySqliteConnectionFactory _factory = default!;
    private SqliteWaveRepository _repo = default!;
    private WavesController _controller = default!;
    private IWaveService _service = default!;

    [SetUp]
    public async Task SetUp()
    {
        // Use shared in-memory database across multiple connections
        _factory = new InMemorySqliteConnectionFactory("Data Source=waves_db;Mode=Memory;Cache=Shared");
        await SchemaHelper.EnsureSchemaAsync(_factory);
        _repo = new SqliteWaveRepository(_factory);
        _service = new WaveService(_repo); // Assuming WaveService implements IWaveService and takes IWaveRepository
        _controller = new WavesController(_service);
    }

    [TearDown]
    public void TearDown()
    {
        if (_factory is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Test]
    public async Task GetAll_ReturnsOk_WithCollection()
    {
        await _repo.UpsertAsync(new Wave { Name = "CtrlA", WaveDate = DateTime.UtcNow });
        await _repo.UpsertAsync(new Wave { Name = "CtrlB", WaveDate = DateTime.UtcNow });

        var actionResult = await _controller.GetAll(CancellationToken.None);
        Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)actionResult.Result!;
        var waves = (IEnumerable<Wave>)ok.Value!;
        Assert.That(waves.Count(), Is.EqualTo(2));
    }
}