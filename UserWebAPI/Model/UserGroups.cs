using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserWebAPI.Model
{
    public class UserGroups
    {
        [Key]
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupMembersIds { get; set; }
    }
}
