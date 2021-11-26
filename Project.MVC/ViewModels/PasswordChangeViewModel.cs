using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage ="{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage = "{0}, must be at least {1} characters!")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [MinLength(4, ErrorMessage = "{0}, must be at least {1} characters!")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "{0}, must be at least {1} characters!")]
        [Compare("NewPassword", ErrorMessage = "{0} and {1} does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
