using Xunit;

namespace DddDemo.EntityFrameworkCore;

[CollectionDefinition(DddDemoTestConsts.CollectionDefinitionName)]
public class DddDemoEntityFrameworkCoreCollection : ICollectionFixture<DddDemoEntityFrameworkCoreFixture>
{

}
