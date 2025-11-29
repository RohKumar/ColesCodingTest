
using ApiRefactor.Domain;
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Infrastructure;

public class SqliteWaveRepository : IWaveRepository
{
    private readonly ISqlConnectionFactory _factory;

    public SqliteWaveRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Wave>> GetAllAsync(CancellationToken ct = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(ct);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, wavedate FROM waves";
        var result = new List<Wave>();

        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Wave
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                WaveDate = reader.GetDateTime(reader.GetOrdinal("wavedate"))
            });
        }
        return result;
    }

    public async Task<Wave?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(ct);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, name, wavedate FROM waves WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return new Wave
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                WaveDate = reader.GetDateTime(reader.GetOrdinal("wavedate"))
            };
        }
        return null;
    }

    public async Task UpsertAsync(Wave wave, CancellationToken ct = default)
    {
        using var connection = _factory.Create();
        await connection.OpenAsync(ct);

        // Try update first, then insert if no rows affected
        using var update = connection.CreateCommand();
        update.CommandText = "UPDATE waves SET name = $name, wavedate = $wavedate WHERE id = $id";
        update.Parameters.AddWithValue("$id", wave.Id);
        update.Parameters.AddWithValue("$name", wave.Name ?? string.Empty);
        update.Parameters.AddWithValue("$wavedate", wave.WaveDate);
        var updated = await update.ExecuteNonQueryAsync(ct);

        if (updated == 0)
        {
            using var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO waves (id, name, wavedate) VALUES ($id, $name, $wavedate)";
            insert.Parameters.AddWithValue("$id", wave.Id);
            insert.Parameters.AddWithValue("$name", wave.Name ?? string.Empty);
            insert.Parameters.AddWithValue("$wavedate", wave.WaveDate);
            await insert.ExecuteNonQueryAsync(ct);
        }
    }
}