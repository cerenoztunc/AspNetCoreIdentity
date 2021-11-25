using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = signUpViewModel.UserName,
                    Email = signUpViewModel.Email,
                    PhoneNumber = signUpViewModel.PhoneNumber
                };
                IdentityResult result = await _userManager.CreateAsync(appUser,signUpViewModel.Password);
                if (result.Succeeded) return RedirectToAction("LogIn");
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(signUpViewModel);
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel logInViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(logInViewModel.Email);
                if (user != null)
                {
                    //await _signInManager.SignOutAsync();
                    var signInResult = await _signInManager.PasswordSignInAsync(user, logInViewModel.Password, true, false);

                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or password is incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No such user found!");
                }
            }
            return View();
        }
    }
}
