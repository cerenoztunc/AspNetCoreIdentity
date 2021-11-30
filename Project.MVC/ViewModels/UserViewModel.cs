using Project.MVC.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "{0} is required!")]
        public string UserName { get; set; }
        [Display(Name = "Phone Number")]
        [RegularExpression("^(0(\\d{3}) (\\d{3}) (\\d{2}) (\\d{2}))$",ErrorMessage = "The phone number is not in the correct format.Please enter as 11 digits, prefixed with 0.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [EmailAddress(ErrorMessage = "{0} is not in the correct format!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "{0}, must be at least {1} characters!")]
        public string Password { get; set; }
        [Display(Name = "Birth Day")]
        [DataType(DataType.Date, ErrorMessage = "{0} is not in the correct format!")]
        public DateTime? BirthDay { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Picture { get; set; }

    }
}
