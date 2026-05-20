using DddDemo.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DddDemo.Permissions;

public class DddDemoPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DddDemoPermissions.GroupName);

        var ordersPermission = myGroup.AddPermission(DddDemoPermissions.Orders.Default, L("Permission:Orders"));
        ordersPermission.AddChild(DddDemoPermissions.Orders.Create, L("Permission:Orders.Create"));
        ordersPermission.AddChild(DddDemoPermissions.Orders.Update, L("Permission:Orders.Update"));
        ordersPermission.AddChild(DddDemoPermissions.Orders.Delete, L("Permission:Orders.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DddDemoResource>(name);
    }
}
