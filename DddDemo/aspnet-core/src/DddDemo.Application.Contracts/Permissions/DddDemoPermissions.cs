namespace DddDemo.Permissions;

public static class DddDemoPermissions
{
    public const string GroupName = "DddDemo";

    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}
