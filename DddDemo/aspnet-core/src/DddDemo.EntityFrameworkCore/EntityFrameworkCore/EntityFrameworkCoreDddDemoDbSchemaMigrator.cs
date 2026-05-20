using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DddDemo.Data;
using Volo.Abp.DependencyInjection;

namespace DddDemo.EntityFrameworkCore;

public class EntityFrameworkCoreDddDemoDbSchemaMigrator
    : IDddDemoDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDddDemoDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the DddDemoDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DddDemoDbContext>()
            .Database
            .MigrateAsync();
    }
}
