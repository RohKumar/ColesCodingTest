using ApiRefactor.Domain;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace ApiRefactor.Infrastructure;
/// <summary>
/// Implements <see cref="IWaveRepository"/> for SQLite.
/// </summary>
public class SqliteWaveRepository : IWaveRepository
{
    private readonly ISqlConnectionFactory _factory;

    public SqliteWaveRepository(ISqlConnectionFactory factory) => _factory = factory;
    /// <summary>
    /// Retrieves all wave entities from the database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Wave>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(cancellationToken);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, wavedate FROM waves";
        var result = new List<Wave>();

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetGuid(reader.GetOrdinal("id"));
            var name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name"));
            var waveDateText = reader.GetString(reader.GetOrdinal("wavedate"));
            var waveDate = DateTime.Now;

            result.Add(new Wave
            {
                Id = id,
                Name = name,
                WaveDate = waveDate
            });
        }
        return result;
    }
    /// <summary>
    /// Retrieves a wave entity by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(cancellationToken);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, wavedate FROM waves WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name"));
            var waveDateText = reader.GetString(reader.GetOrdinal("wavedate"));
            var waveDate = DateTime.Parse(waveDateText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            return new Wave
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = name,
                WaveDate = waveDate
            };
        }
        return null;
    }
    /// <summary>
    /// / Upserts a wave entity into the database.  
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpsertAsync(Wave wave, CancellationToken cancellationToken = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(cancellationToken);

        // Ensure a non-empty Id for inserts/updates
        if (wave.Id == Guid.Empty)
        {
            wave.Id = Guid.NewGuid();
        }

        // Try update first, then insert if no rows affected
        using var update = connection.CreateCommand();
        update.CommandText = "UPDATE waves SET name = $name, wavedate = $wavedate WHERE id = $id";
        update.Parameters.AddWithValue("$id", wave.Id);
        update.Parameters.AddWithValue("$name", wave.Name ?? string.Empty);
        update.Parameters.AddWithValue("$wavedate", wave.WaveDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture));
        var updated = await update.ExecuteNonQueryAsync(cancellationToken);

        if (updated == 0)
        {
            using var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO waves (id, name, wavedate) VALUES ($id, $name, $wavedate)";
            insert.Parameters.AddWithValue("$id", wave.Id);
            insert.Parameters.AddWithValue("$name", wave.Name ?? string.Empty);
            insert.Parameters.AddWithValue("$wavedate", wave.WaveDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture));
            await insert.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}