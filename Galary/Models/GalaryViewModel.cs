using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Galary.Models
{
    public class GalaryViewModel
    {
        [Display(Name="Video Type :")]
        public List<VideoTypes> VideoTypes { get; set; }

        public int SelectedVideoType { get; set; }

        public List<Videos> Videos { get; set; }
    }
}