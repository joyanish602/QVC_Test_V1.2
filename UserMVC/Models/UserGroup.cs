using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserMVC.Models
{
    public class UserGroup
    {
        public int Id { get; set; }

        [DisplayName("Group Name")]
        public string GroupName { get; set; }

        public string GroupMembersIds { get; set; }

        [DisplayName("Members")]
        public string GroupMembersNames { get; set; }
    }
}
