using Microsoft.Extensions.Localization;
using DddDemo.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DddDemo;

[Dependency(ReplaceServices = true)]
public class DddDemoBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DddDemoResource> _localizer;

    public DddDemoBrandingProvider(IStringLocalizer<DddDemoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
