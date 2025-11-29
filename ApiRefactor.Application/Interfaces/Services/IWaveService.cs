
using ApiRefactor.Domain;

namespace ApiRefactor.Application;

public interface IWaveService
{
    Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default);
    Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpsertAsync(Wave wave, CancellationToken ct = default);
}