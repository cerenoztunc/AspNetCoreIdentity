using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class AdminChangePasswordViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "{0}, must be at least {1} characters!")]
        public string Password { get; set; }
    }
}
