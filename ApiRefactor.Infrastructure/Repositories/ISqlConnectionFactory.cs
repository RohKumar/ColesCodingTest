
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Infrastructure;

public interface ISqlConnectionFactory
{
    SqliteConnection Create();
}