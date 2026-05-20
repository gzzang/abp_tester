using Volo.Abp.Modularity;

namespace DddDemo;

[DependsOn(
    typeof(DddDemoDomainModule),
    typeof(DddDemoTestBaseModule)
)]
public class DddDemoDomainTestModule : AbpModule
{

}
