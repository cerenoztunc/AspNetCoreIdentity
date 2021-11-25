using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required(ErrorMessage = "{0} is required!")]
        [EmailAddress(ErrorMessage = "{0} is not in the correct format!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "{0}, must be at least {1} characters!")]
        public string NewPassword { get; set; }
    }
}
