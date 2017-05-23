using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Galary.Models
{
    public class RolesViewModel
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public bool? IsSelected { get; set; }
    }
}