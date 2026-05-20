using DddDemo.Samples;
using Xunit;

namespace DddDemo.EntityFrameworkCore.Applications;

[Collection(DddDemoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DddDemoEntityFrameworkCoreTestModule>
{

}
