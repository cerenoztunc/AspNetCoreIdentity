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
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name); //her kullanıcı için bir User.Identity yapısı mutlaka oluşur. kullanıcı kayıt yapmamış olsa da ama burada içi boştur. Eğer kullanıcı kayıt yapmışsa o zaman bilgiler dolmaya başlar..
            UserViewModel userViewModel = user.Adapt<UserViewModel>(); //adapt map gibidir. ondan daha hafiftir. 
            
            return View(userViewModel);
        }
        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (appUser != null)
                {
                    bool exist = await _userManager.CheckPasswordAsync(appUser, passwordChangeViewModel.OldPassword);
                    if (exist)
                    {
                        IdentityResult result = await _userManager.ChangePasswordAsync(appUser, passwordChangeViewModel.OldPassword, passwordChangeViewModel.NewPassword);
                        if (result.Succeeded)
                        {
                            await _userManager.UpdateSecurityStampAsync(appUser);
                            await _signInManager.SignOutAsync();
                            await _signInManager.PasswordSignInAsync(appUser, passwordChangeViewModel.NewPassword,true,false);
                            ViewBag.success = "true";
                        }
                        else
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("", item.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "OldPassword is wrong!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is not found");
                }
            }
            else
            {
                return View(passwordChangeViewModel);
            }
            return View(passwordChangeViewModel);

        }
        public IActionResult UserEdit()
        {
            AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            UserViewModel userViewModel = user.Adapt<UserViewModel>();
            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel)
        {
            ModelState.Remove("Password");
            
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.UserName = userViewModel.UserName;
                user.PhoneNumber = userViewModel.PhoneNumber;
                user.Email = userViewModel.Email;
                IdentityResult result =  await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);
                    ViewBag.success = "true";
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(userViewModel);
        }
        public void LogOut()
        {
            _signInManager.SignOutAsync();
        }
    }
}
