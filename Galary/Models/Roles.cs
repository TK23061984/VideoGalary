using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Galary.Models
{
    public class Roles
    {
        [Key]
        public int RoleID { get; set; }

        [Required(ErrorMessage="Please enter Role Name" , AllowEmptyStrings=false)]
        [Display(Name="Role Name")]
        public string RoleName { get; set; }

        [Display(Name="Role Description")]
        public string RoleDescription { get; set; }

        public virtual ICollection<UsersRoles> AllowedUsers { get; set; }

        public virtual ICollection<VideoTypeRoles> AllowedVideoTypes { get; set; }
    }
}