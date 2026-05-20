using DddDemo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DddDemo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DddDemoController : AbpControllerBase
{
    protected DddDemoController()
    {
        LocalizationResource = typeof(DddDemoResource);
    }
}
