using ApiRefactor.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace ApiRefactor.Tests.Support
{
    /// <summary>
    /// Provides a factory for creating open SQLite in-memory database connections that share the same database instance
    /// for the lifetime of the factory.
    /// </summary>
    
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
        /// <summary>
        /// Creates and opens a new connection to the SQLite database using the configured connection string.
        /// </summary>
        /// <remarks>Returns an open SQLite connection; you must dispose it.</remarks>
        /// <returns>A <see cref="SqliteConnection"/> instance that is already open and ready for database operations.</returns>
        public SqliteConnection Create()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
        /// <summary>
        /// Disposes the in-memory SQLite connection factory and its resources.
        /// </summary>
        public void Dispose()
        {
            _keeper.Dispose();
        }
    }
}