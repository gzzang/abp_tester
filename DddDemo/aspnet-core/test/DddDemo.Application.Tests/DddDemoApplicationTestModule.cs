using Volo.Abp.Modularity;

namespace DddDemo;

[DependsOn(
    typeof(DddDemoApplicationModule),
    typeof(DddDemoDomainTestModule)
)]
public class DddDemoApplicationTestModule : AbpModule
{

}
