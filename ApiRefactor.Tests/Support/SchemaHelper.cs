using Microsoft.Data.Sqlite;
using ApiRefactor.Tests.Support;

namespace ApiRefactor.Tests.Support;
/// <summary>
/// Provides methods for ensuring the database schema is created.
/// </summary>
public static class SchemaHelper
{   /// <summary>
    /// Ensures the database schema is created.
    /// </summary>
    /// <param name="factory">The SQLite connection factory.</param>
    public static async Task EnsureSchemaAsync(InMemorySqliteConnectionFactory factory)
    {
        using var conn = (SqliteConnection)factory.Create();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        // wavedate stored as TEXT (ISO 8601) so repository GetDateTime works
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS waves(
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NULL,
                wavedate TEXT NOT NULL
            );
            """;
        await cmd.ExecuteNonQueryAsync();
    }
}