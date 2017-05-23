using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galary.Models
{
    [Table("UsersRoles")]
    public class UsersRoles
    {
        [Key]
        [Column(Order = 1)]
        public int UserID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RoleID { get; set; }

        public virtual Users Users { get; set; }

        public virtual Roles Roles { get; set; }
    }
}