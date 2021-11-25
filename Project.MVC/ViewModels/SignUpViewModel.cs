using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="{0} is required!")]
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="{0} is required!")]
        [EmailAddress(ErrorMessage = "{0} is not in the correct format!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(3,ErrorMessage = "{0}, must be at least {1} characters!")]
        public string Password { get; set; }
    }
}
