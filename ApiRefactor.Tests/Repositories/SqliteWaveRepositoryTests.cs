using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;
using ApiRefactor.Tests.Support;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace ApiRefactor.Tests.Repositories;

[TestFixture]
public class SqliteWaveRepositoryTests
{
    private ApiRefactor.Infrastructure.ISqlConnectionFactory _factory = default!;
    private SqliteWaveRepository _repo = default!;

    [SetUp]
    public async Task SetUp()
    {
        // Provide a connection string as required by the constructor
        var connectionString = "Data Source=waves;Mode=Memory;Cache=Shared";
        var factory = new ApiRefactor.Tests.Support.InMemorySqliteConnectionFactory(connectionString);
        await SchemaHelper.EnsureSchemaAsync(factory);
        _factory = factory;
        _repo = new SqliteWaveRepository(_factory);
    }

    [TearDown]
    public void TearDown()
    {
        if (_factory is IDisposable disposable)
            disposable.Dispose();
    }

    [Test]
    public async Task Upsert_Inserts_WhenMissing()
    {
        var wave = new Wave { Name = "Alpha", WaveDate = DateTime.UtcNow };
        await _repo.UpsertAsync(wave);

        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.Name, Is.EqualTo("Alpha"));
    }

    [Test]
    public async Task Upsert_Updates_WhenExists()
    {
        var wave = new Wave { Name = "Initial", WaveDate = DateTime.UtcNow };
        await _repo.UpsertAsync(wave);

        wave.Name = "Updated";
        wave.WaveDate = wave.WaveDate.AddMinutes(5);
        await _repo.UpsertAsync(wave);

        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched!.Name, Is.EqualTo("Updated"));
        Assert.That(fetched.WaveDate, Is.EqualTo(wave.WaveDate));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repo.GetByIdAsync(Guid.NewGuid());
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ReturnsMultiple_Waves()
    {
        var w1 = new Wave { Name = "One", WaveDate = DateTime.UtcNow };
        var w2 = new Wave { Name = "Two", WaveDate = DateTime.UtcNow.AddHours(1) };
        await _repo.UpsertAsync(w1);
        await _repo.UpsertAsync(w2);

        var all = (await _repo.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(2));
        Assert.That(all.Select(w => w.Name), Is.EquivalentTo(new[] { "One", "Two" }));
    }

    [Test]
    public async Task FractionalSeconds_Preserved_OnRead()
    {
        var precise = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddTicks(3456701);
        var wave = new Wave { Name = "Precise", WaveDate = precise };
        await _repo.UpsertAsync(wave);

        var fetched = await _repo.GetByIdAsync(wave.Id);
        // Because SQLite stores as text ISO 8601, ticks beyond millisecond may truncate; assert millisecond precision
        Assert.That(fetched!.WaveDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
            Is.EqualTo(precise.ToString("yyyy-MM-dd HH:mm:ss.fff")));
    }

    [Test]
    public async Task Name_Defaults_ToEmptyString_WhenNullPassed()
    {
        var wave = new Wave { Name = null!, WaveDate = DateTime.UtcNow };
        await _repo.UpsertAsync(wave);
        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched!.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Upsert_Respects_Cancellation()
    {
        var wave = new Wave { Name = "Cancel", WaveDate = DateTime.UtcNow };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.That(async () => await _repo.UpsertAsync(wave, cts.Token),
            Throws.TypeOf<OperationCanceledException>().Or.TypeOf<TaskCanceledException>());
    }
}