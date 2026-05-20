using Volo.Abp.Modularity;

namespace DddDemo;

public abstract class DddDemoApplicationTestBase<TStartupModule> : DddDemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
