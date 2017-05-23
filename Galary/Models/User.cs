using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Galary.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage="Please enter User Name", AllowEmptyStrings = false)]
        [Display(Name="User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage="Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Display(Name="Last Name")]
        public string LastName { get; set; }

        [Display(Name="Email ID")]
        [EmailAddress(ErrorMessage="Invalid Email ID")]
        public string EmailID { get; set; }

        [Display(Name="Mobile No")]
        [StringLength(10, ErrorMessage = "The Mobile must have 10 numbers", MinimumLength = 10)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "The Mobile must have 10 numbers")]
        public string Mobile { get; set; }

        public DateTime CreatedDate { get; set; }
        
        [Display(Name="Active")]
        public bool IsActive { get; set; }

        public virtual ICollection<UsersRoles> AssignedRoles { get; set; }
    }
}