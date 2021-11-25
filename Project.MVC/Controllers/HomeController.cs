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
        public IActionResult LogIn(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
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
                    if(await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Your account has been locked for a while. Please try again later!");
                    }
                    await _signInManager.SignOutAsync();
                    var signInResult = await _signInManager.PasswordSignInAsync(user, logInViewModel.Password, logInViewModel.RememberMe, false);

                    if (signInResult.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);
                        
                        int fail = await _userManager.GetAccessFailedCountAsync(user);
                        if (fail<3)
                        {
                            ModelState.AddModelError("", $"{fail} times failed login!");
                            ModelState.AddModelError("", "Username or password is incorrect");
                        }
                        
                        else if (fail == 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                            ModelState.AddModelError("", "Your account has been locked for 20 minutes due to 3 failed logins!");
                        }
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No such user found!");
                }
            }
            return View(logInViewModel);
        }
    }
}
