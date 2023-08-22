using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserMVC.Models
{
    public class UserGroupResp
    {
        public int Id { get; set; }

        [DisplayName("Group Name")]
        [Required]
        public string GroupName { get; set; }

        public List<SelectListItem> GroupMembers { get; set; }

        [DisplayName("Users")]
        [Required]
        public long[] GroupMembersIds { get; set; }
    }
}
