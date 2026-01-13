
namespace EleonCore.Modules.S3.Permissions;

public static class S3Permissions
{
    public const string GroupName = "S3";

    public static class Buckets
    {
        public const string Default = GroupName + ".Buckets";
        public const string Manage = Default + ".Manage";
        public const string Read = Default + ".Read";
        public const string Write = Default + ".Write";
    }

    public static class Objects
    {
        public const string Default = GroupName + ".Objects";
        public const string Read = Default + ".Read";
        public const string Write = Default + ".Write";
        public const string Delete = Default + ".Delete";
    }

    public static class Multipart
    {
        public const string Default = GroupName + ".Multipart";
        public const string Initiate = Default + ".Initiate";
        public const string Complete = Default + ".Complete";
    }
}
