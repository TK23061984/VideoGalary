using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Galary.Models
{
    public class UsersViewModel
    {
        public Users User { get; set; }

        public List<RolesViewModel> AllRoles { get; set; }

        [Required(ErrorMessage="Please select atleast one user role")]
        public List<int> SelectedRoles { get; set; }
    }
}