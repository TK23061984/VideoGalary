using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Galary.Models
{
    public class VideosViewModel
    {
        [Key]
        public int VideoID { get; set; }

        [Required(ErrorMessage = "Please select Video Type")]
        [Display(Name = "Video Type")]
        public int VideoTypeID { get; set; }

        [Display(Name = "Video Description")]
        public string VideoDescription { get; set; }

        [Required(ErrorMessage = "Please enter Video ID", AllowEmptyStrings = false)]
        [Display(Name = "Video ID")]
        public string VideoURL { get; set; }

        public List<VideoTypes> VideoTypeList { get; set; }
    }
}