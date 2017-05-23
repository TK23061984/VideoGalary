using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Galary.Models
{
    public class LoginViewModel
    {
        [Key]
        [Required(ErrorMessage="Please enter User Name", AllowEmptyStrings=false)]
        [Display(Name="User Name")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage="Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}