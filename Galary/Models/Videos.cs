using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galary.Models
{
    [Table("Videos")]
    public class Videos
    {
        [Key]
        public int VideoID { get; set; }

        [Required]
        public int VideoTypeID { get; set; }

        public string VideoDescription { get; set; }

        [Required]
        [Display(Name="Video ID")]
        public string VideoURL { get; set; }

        public string VideoImage { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}