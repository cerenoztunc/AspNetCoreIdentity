using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ViewModels
{
    public class RoleAssignViewModel
    { 
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Exist { get; set; }
    }
}
