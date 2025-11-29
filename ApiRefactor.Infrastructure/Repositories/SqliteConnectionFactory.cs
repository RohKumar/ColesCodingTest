
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Infrastructure;

public class SqliteConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    public SqliteConnectionFactory(string connectionString) => _connectionString = connectionString;
    public SqliteConnection Create() => new SqliteConnection(_connectionString);
}