using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class RoleViewModel
    {
        [Display(Name="User Name")]
        [Required(ErrorMessage ="{0} is required!")]
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
