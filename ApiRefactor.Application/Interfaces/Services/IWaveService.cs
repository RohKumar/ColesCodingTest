
using ApiRefactor.Domain;

namespace ApiRefactor.Application;
/// <summary>
/// Defines methods for retrieving and managing wave entities asynchronously.
/// </summary>
/// <remarks>Every method is async and supports cancellation with a CancellationToken.</remarks>
public interface IWaveService
{
    Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default);
    Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpsertAsync(Wave wave, CancellationToken ct = default);
}