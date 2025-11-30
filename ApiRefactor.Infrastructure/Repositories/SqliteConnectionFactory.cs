
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Infrastructure;
/// <summary>
/// Defines a factory for creating new instances of <see cref="SqliteConnection"/> for use with SQLite databases.
/// </summary>
/// <remarks>
/// Implementations configure and initialize <see cref="SqliteConnection"/> instances as needed.
/// Returned connections are closed by default; callers must open and dispose them.
/// </remarks>
public class SqliteConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    public SqliteConnectionFactory(string connectionString) => _connectionString = connectionString;
    public SqliteConnection Create() => new SqliteConnection(_connectionString);
}