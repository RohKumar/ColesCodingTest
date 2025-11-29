using ApiRefactor.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace ApiRefactor.Tests.Support
{
    public sealed class InMemorySqliteConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _keeper;

        public InMemorySqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
            _keeper = new SqliteConnection(_connectionString);
            _keeper.Open();
        }

        public SqliteConnection Create()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void Dispose()
        {
            _keeper.Dispose();
        }
    }
}