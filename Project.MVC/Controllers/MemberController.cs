using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
namespace Project.MVC.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public MemberController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name); //her kullanıcı için bir User.Identity yapısı mutlaka oluşur. kullanıcı kayıt yapmamış olsa da ama burada içi boştur. Eğer kullanıcı kayıt yapmışsa o zaman bilgiler dolmaya başlar..
            UserViewModel userViewModel = user.Adapt<UserViewModel>(); //adapt map gibidir. ondan daha hafiftir. 
            
            return View(userViewModel);
        }
    }
}
