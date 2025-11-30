
using ApiRefactor.Domain;

namespace ApiRefactor.Infrastructure;
/// <summary>
/// Defines the contract for a repository that manages wave entities.
/// </summary>
public interface IWaveRepository
{
    Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default);
    Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpsertAsync(Wave wave, CancellationToken ct = default);
}