
using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;

namespace ApiRefactor.Application;

/// <summary>
/// Provides operations for retrieving and managing wave entities.
/// </summary>
/// <remarks>Application service for managing waves. Wraps the repository and provides async methods used by business logic or APIs.</remarks>
public class WaveService : IWaveService
{
    private readonly IWaveRepository _repo;
    public WaveService(IWaveRepository repo) => _repo = repo;

    public Task<IEnumerable<Wave>> GetAllAsync(CancellationToken cancellationToken = default) => _repo.GetAllAsync(cancellationToken);
    public Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => _repo.GetByIdAsync(id, cancellationToken);
    public Task UpsertAsync(Wave wave, CancellationToken cancellationToken = default) => _repo.UpsertAsync(wave, cancellationToken);
}