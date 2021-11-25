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
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel passwordResetViewModel)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(passwordResetViewModel.Email);

            if (appUser != null)
            {
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = appUser.Id,
                    token = passwordResetToken,
                }, HttpContext.Request.Scheme);

                Helpers.PasswordReset.PasswordResetSendEmail(passwordResetLink);

                ViewBag.status = "successfull";
            }
            else
            {
                ModelState.AddModelError("", "No such user found!");
            }
            return View(passwordResetViewModel);
        }
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("NewPassword")]PasswordResetViewModel passwordResetViewModel) //bind view içinde sadece istediğimiz property'i almamızı sağlar..
        {
            string token = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();

            AppUser user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, passwordResetViewModel.NewPassword);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user); //kritik bilgiler güncellendiğinde securitystamp alanı update edilmelidir. yoksa kullanıcı eski şifresini kullanabilmeye devam eder.
                    
                    ViewBag.status = "successfull";
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
                ModelState.AddModelError("", "An error has occurred. Please try again later.");
            }
            return View();
        }

    }
}
