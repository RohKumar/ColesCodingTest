
using ApiRefactor.Domain;

namespace ApiRefactor.Infrastructure;

public interface IWaveRepository
{
    Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default);
    Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpsertAsync(Wave wave, CancellationToken ct = default);
}