
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Infrastructure;
/// <summary>
/// Defines a factory for creating new instances of <see cref="SqliteConnection"/> for use with SQLite databases.
/// </summary>
/// <remarks>Implementations configure and initialize <see cref="SqliteConnection"/> instances. 
/// Returned connections are closed; callers must open and dispose them.
/// </remarks>
public interface ISqlConnectionFactory
{
    SqliteConnection Create();
}