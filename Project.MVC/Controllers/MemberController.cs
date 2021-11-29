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
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.MVC.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace Project.MVC.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        private readonly IWebHostEnvironment _hostWebEnvironment;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment hostWebEnvironment):base(userManager,signInManager)
        {
            _hostWebEnvironment = hostWebEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
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
            AppUser appUser = CurrentUser;
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
                            AddModelError(result);
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
            AppUser user = CurrentUser;
            UserViewModel userViewModel = user.Adapt<UserViewModel>();

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel,IFormFile userPicture)
        {
            ModelState.Remove("Password");
            
            if (ModelState.IsValid)
            {
                AppUser user = CurrentUser;
                if (userPicture != null)
                {
                    if (user.Picture == null)
                    {
                        if (userPicture != null && userPicture.Length > 0)
                        {
                            await UploadImage(userPicture, user);
                        }
                    }
                    else 
                    {
                        if (user.Picture != "profile.jpg")
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", user.Picture);
                            var toBeDeleted = oldPath.Split("/")[2];
                            var fullPath = _hostWebEnvironment.WebRootPath + "/UserPicture/" + toBeDeleted;
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            await UploadImage(userPicture, user);
                        }
                    }
                }
                user.City = userViewModel.City.ToLower();
                user.BirthDay = userViewModel.BirthDay;
                user.Gender = (int)userViewModel.Gender;
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
                    AddModelError(result);
                }
            }
            return View(userViewModel);
        }
        public async Task UploadImage(IFormFile userPicture, AppUser user)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await userPicture.CopyToAsync(stream);
                user.Picture = "/UserPicture/" + fileName;
            }
        }
        public void LogOut()
        {
            _signInManager.SignOutAsync();
        }
        public IActionResult AccessDenied(string returnUrl)
        {
            if (returnUrl.Contains("ViolancePage"))
                ViewBag.message = "Since the page you are trying to access contains videos of violence, you must be over the age of 15.";
            else if (returnUrl.Contains("AnkaraPage"))
                ViewBag.message = "Only users whose city area is Ankara can access this page.";
            else if (returnUrl.Contains("Exchange"))
                ViewBag.message = "Your free 30-day usage has expired!";
            else
                ViewBag.message = "You are not authorized to enter this page! Please contact the site administrator to obtain authorization.";
            return View();
        }
        [Authorize(Roles ="Editor,Admin")]
        public IActionResult Editor()
        {
            return View();
        }
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Manager()
        {
            return View();
        }
        [Authorize(Policy="AnkaraPolicy")]
        public IActionResult AnkaraPage()
        {
            return View();
        }
        [Authorize(Policy ="ViolancePolicy")]
        public IActionResult ViolancePage()
        {
            return View();
        }
        public async Task<IActionResult> ExchangeRedirect()
        {
            if (!User.HasClaim(x => x.Type == "ExpireDate"))
            {
                Claim claim = new Claim("ExpireDate", DateTime.Now.AddDays(30).Date.ToShortDateString(), ClaimValueTypes.String, "Internal");
                await _userManager.AddClaimAsync(CurrentUser, claim);

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(CurrentUser, true);
            }
                
            return RedirectToAction("Exchange");
        }
        [Authorize(Policy = "ExpiryDatePolicy")]
        public IActionResult Exchange()
        {
            return View();
        }
    }
}
