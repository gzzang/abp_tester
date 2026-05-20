using System.Threading.Tasks;

namespace DddDemo.Data;

public interface IDddDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
