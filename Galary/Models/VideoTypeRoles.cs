using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galary.Models
{
    [Table("VideoTypeRoles")]
    public class VideoTypeRoles
    {
        [Key]
        [Column(Order = 1)]
        public int VideoTypeID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RoleID { get; set; }

        public virtual VideoTypes VideoTypes { get; set; }

        public virtual Roles Roles { get; set; }
    }
}