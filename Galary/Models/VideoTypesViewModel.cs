using System.Collections.Generic;

namespace Galary.Models
{
    public class VideoTypesViewModel
    {
        public VideoTypes VideoTypes { get; set; }

        public List<RolesViewModel> AllRoles { get; set; }

        public List<int> SelectedRoles { get; set; }
    }
}