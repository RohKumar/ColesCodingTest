using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;
using ApiRefactor.Tests.Support;
using NUnit.Framework;

namespace ApiRefactor.Tests.Repositories;
/// <summary>
/// Provides unit tests for the SqliteWaveRepository
/// </summary>
/// <remarks>Unit tests for SQLite repo: CRUD, fractional seconds, defaults, cancellation on in‑memory DB.</remarks>
[TestFixture]
public class SqliteWaveRepositoryTests
{
    private ApiRefactor.Infrastructure.ISqlConnectionFactory _factory = default!;
    private SqliteWaveRepository _repo = default!;
    /// <summary>
    /// Initializes a new in-memory SQLite database and repository instance before each test is executed.   
    /// Sets up a new in-memory SQLite database and repository instance before each test
    /// </summary>
    /// <remarks>Fresh in-memory SQLite DB per test; ensures isolation.</remarks>
    /// <returns></returns>
    [SetUp]
    public async Task SetUp()
    {
        // Provide a connection string as required by the constructor
        var connectionString = "Data Source=waves;Mode=Memory;Cache=Shared";
        var factory = new InMemorySqliteConnectionFactory(connectionString);
        await SchemaHelper.EnsureSchemaAsync(factory);
        _factory = factory;
        _repo = new SqliteWaveRepository(_factory);
    }
    /// <summary>
    /// Cleans up resources after each test is executed.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (_factory is IDisposable disposable)
            disposable.Dispose();
    }
    /// <summary>
    /// Tests the UpsertAsync method of the repository.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Upsert_Inserts_WhenMissing()
    {
        var wave = new Wave { Name = "Alpha", WaveDate = DateTime.UtcNow };
        await _repo.UpsertAsync(wave);

        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.Name, Is.EqualTo("Alpha"));
    }
    /// <summary>
    /// Tests the UpsertAsync method of the repository.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Tests the GetByIdAsync method of the repository.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repo.GetByIdAsync(Guid.NewGuid());
        Assert.That(result, Is.Null);
    }
    /// <summary>
    /// Tests the GetAllAsync method of the repository.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Tests the FractionalSeconds_Preserved_OnRead method of the repository.
    /// </summary>
    /// <returns></returns>

    [Test]
    public async Task FractionalSeconds_Preserved_OnRead()
    {
        var precise = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc).AddTicks(3456701);
        var wave = new Wave { Name = "Precise", WaveDate = precise };
        await _repo.UpsertAsync(wave);

        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched!.WaveDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
            Is.EqualTo(precise.ToString("yyyy-MM-dd HH:mm:ss.fff")));
    }
    /// <summary>
    /// Tests the Name_Defaults_ToEmptyString_WhenNullPassed method of the repository.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Name_Defaults_ToEmptyString_WhenNullPassed()
    {
        var wave = new Wave { Name = null!, WaveDate = DateTime.UtcNow };
        await _repo.UpsertAsync(wave);
        var fetched = await _repo.GetByIdAsync(wave.Id);
        Assert.That(fetched!.Name, Is.EqualTo(string.Empty));
    }
    /// <summary>
    /// Tests the Upsert_Respects_Cancellation method of the repository.
    /// </summary>
    [Test]
    public void Upsert_Respects_Cancellation()
    {
        var wave = new Wave { Name = "Cancel", WaveDate = DateTime.UtcNow };
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        Assert.That(async () => await _repo.UpsertAsync(wave, cancellationTokenSource.Token),
            Throws.TypeOf<OperationCanceledException>().Or.TypeOf<TaskCanceledException>());
    }
}