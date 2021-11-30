using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace Project.MVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment) : base(userManager, signInManager)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel signUpViewModel)
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
                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    var link = Url.Action("EmailConfirm", "Home", new
                    {
                        userId = appUser.Id,
                        token = confirmationToken
                    },HttpContext.Request.Scheme);
                    Helpers.EmailConfirmation.EmailConfirmationSendEmail(link, appUser.Email);
                    ViewBag.status = "true";
                    return View(signUpViewModel);
                }
                else
                {
                    AddModelError(result);
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
                    if(user.EmailConfirmed == true)
                    {
                        if (await _userManager.IsLockedOutAsync(user))
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
                            if (fail < 4)
                            {
                                ModelState.AddModelError("", $"{fail} times failed login!");
                                ModelState.AddModelError("", "Username or password is incorrect");
                            }

                            else
                            {
                                await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                                ModelState.AddModelError("", "Your account has been locked for 20 minutes due to 3 failed logins!");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your email address has not been verified.Please check your email address!");
                        return View(logInViewModel);
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

                Helpers.PasswordReset.PasswordResetSendEmail(passwordResetLink,appUser.Email);

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
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "An error has occurred. Please try again later.");
            }
            return View();
        }
        public async Task<IActionResult> EmailConfirm(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.status = true;
            }

            else ViewBag.status = false;

            return View();
        }
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new
            {
                ReturnUrl = ReturnUrl
            }); //kullanıcının facebook giriş sayfasındaki işlemlerini yaptıktan sonra gidecek olduğu url'i oluşturduk..
            var property = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl); //nereye gideceği(Facebook) ve nereye döneceğini(döneceği sayfa) belirttik..

            return new ChallengeResult("Facebook", property); //butona tıklandığında dönüş bilgisiyle birlikte facebook login sayfasına yönlendirdik..
        }
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new
            {
                ReturnUrl = ReturnUrl
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);
            return new ChallengeResult("Google", properties);
        }
        public IActionResult MicrosoftLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var property = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", RedirectUrl);
            return new ChallengeResult("Microsoft", property);
        }
        public async Task<IActionResult> ExternalResponse(string ReturnUrl="/") //kullanıcının döneceği sayfa
        {
            //kullanıcının facebook login olduğu ile ilgili bilgileri aldık..
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) //kullanıcı facebook login ekranında bilgilerini vermemiş olabilir..
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                if (result.Succeeded) return Redirect(ReturnUrl);
                else
                {
                    AppUser appUser = new AppUser();
                    //facebooktan gelen email, name, username gibi bilgileri identity api claim bilgilerine dönüştürerek bize sunuyor..principal üzerinden biz bu claimlere erişebiliyoruz. aynı zamanda FindFirst metodu üzerinden de erişiliyor..
                    appUser.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    if (info.Principal.HasClaim(x=>x.Type==ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value; //kullanıcı adını aldık ancak bu isim bize (Ceren Öztunç gibi) arasında boşluklu geliyor..
                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();
                        appUser.UserName = userName;
                    }
                    else
                    {
                        appUser.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    AppUser appUser2 = await _userManager.FindByEmailAsync(appUser.Email);
                    if (appUser2 == null)
                    {
                        IdentityResult createResult = await _userManager.CreateAsync(appUser);
                        if (createResult.Succeeded)
                        {
                            IdentityResult loginResult = await _userManager.AddLoginAsync(appUser, info); //bilgileri userlogins tablosuna kaydettik..Üçüncü taraf kimlik doğrulamalarda bu tablo mutlaka doldurulmalıdır. Yoksa facebook'tan giriş yapıldığını anlayamaz..
                            if (loginResult.Succeeded)
                            {
                                //await _signInManager.SignInAsync(appUser, true); //burada normal bir kullanıcı gibi kayıt ettiğimiz için claim bilgilerinde facebook'tan geldiğine dair bir iz bulamayız.. bunun iin aşağıdaki gibi external signin yapmak gerekir..

                                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true); //böyle yapınca bunun üzerinden claim bazlı yetkilendirme de yapabiliriz. örneğin sadece facebook'tan gelen kullanıcıların görebildiği bir sayfa..
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                AddModelError(loginResult);
                            }
                        }
                        else
                        {
                            AddModelError(createResult);
                        }
                    }
                    else
                    {
                        IdentityResult loginResult = await _userManager.AddLoginAsync(appUser2, info);
                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,true);
                        return Redirect(ReturnUrl);
                    }
                }
            }
            List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();
            return View("Error",errors);
        }
        public IActionResult Error()
        {
            return View();
        }
       

    }
}
