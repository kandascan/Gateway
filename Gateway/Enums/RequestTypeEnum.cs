using System.Runtime.Serialization;

namespace Gateway.Enums
{
    public enum RequestTypeEnum
    {
        [EnumMember(Value = "Product")]
        Product = 1,
        [EnumMember(Value = "Post")]
        Post = 2
    }
}
