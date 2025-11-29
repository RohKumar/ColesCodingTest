
using ApiRefactor.Domain;
using ApiRefactor.Infrastructure;

namespace ApiRefactor.Application;

public class WaveService : IWaveService
{
    private readonly IWaveRepository _repo;
    public WaveService(IWaveRepository repo) => _repo = repo;

    public Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
    public Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default) => _repo.GetByIdAsync(id, ct);
    public Task UpsertAsync(Wave wave, CancellationToken ct = default) => _repo.UpsertAsync(wave, ct);
}