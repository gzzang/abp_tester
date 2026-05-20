using DddDemo.Samples;
using Xunit;

namespace DddDemo.EntityFrameworkCore.Domains;

[Collection(DddDemoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DddDemoEntityFrameworkCoreTestModule>
{

}
