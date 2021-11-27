using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;
        protected readonly RoleManager<AppRole> _roleManager;
        protected AppUser CurrentUser => _userManager.FindByNameAsync(User.Identity.Name).Result; //her kullanıcı için bir User.Identity yapısı mutlaka oluşur. kullanıcı kayıt yapmamış olsa da ama burada içi boştur. Eğer kullanıcı kayıt yapmışsa o zaman bilgiler dolmaya başlar..
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager = null, RoleManager<AppRole> roleManager=null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
    }
}
