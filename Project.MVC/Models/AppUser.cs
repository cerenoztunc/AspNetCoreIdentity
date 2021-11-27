using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.MVC.Enums;

namespace Project.MVC.Models
{
    public class AppUser:IdentityUser
    {
        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDay { get; set; }
        public int Gender { get; set; }
        public AppUser()
        {
            Gender = (int) Enums.Gender.Unspecified;
        }
    }
}
