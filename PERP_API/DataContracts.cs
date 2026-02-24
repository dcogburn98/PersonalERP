using System.Runtime.Serialization;

namespace PERP_API
{
    [DataContract]
    public class UserInfo
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Username { get; set; }
        [DataMember] public string DisplayName { get; set; }
        [DataMember] public string Role { get; set; }
        [DataMember] public bool IsActive { get; set; }
    }

    [DataContract]
    public class ModulePermission
    {
        [DataMember] public string Role { get; set; }
        [DataMember] public string ModuleName { get; set; }
        [DataMember] public bool CanView { get; set; }
        [DataMember] public bool CanEdit { get; set; }
        [DataMember] public bool CanAdmin { get; set; }
    }
}
