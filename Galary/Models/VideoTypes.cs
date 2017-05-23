using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galary.Models
{
    [Table("VideoTypes")]
    public class VideoTypes
    {
        [Key]
        public int VideoTypeID { get; set; }

        [Required(ErrorMessage="Please enter Video Type Name" , AllowEmptyStrings=false)]
        [Display(Name="Video Type Name")]
        public string VideoTypeName { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Videos> VideosCollection { get; set; }

        public virtual ICollection<VideoTypeRoles> AssignedRoles { get; set; }
    }
}