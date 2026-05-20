using DddDemo.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DddDemo.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DddDemoEntityFrameworkCoreModule),
    typeof(DddDemoApplicationContractsModule)
    )]
public class DddDemoDbMigratorModule : AbpModule
{
}
