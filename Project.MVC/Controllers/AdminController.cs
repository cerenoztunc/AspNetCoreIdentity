using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
    }
}
