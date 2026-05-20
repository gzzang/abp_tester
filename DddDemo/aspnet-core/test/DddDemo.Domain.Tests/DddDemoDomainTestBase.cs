using Volo.Abp.Modularity;

namespace DddDemo;

/* Inherit from this class for your domain layer tests. */
public abstract class DddDemoDomainTestBase<TStartupModule> : DddDemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
