using Mapster;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles ="Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager):base(userManager,null,roleManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }
        public IActionResult Roles()
        {
            
            return View(_roleManager.Roles.ToList());
        }
        public IActionResult Claims()
        {
            return View(User.Claims.ToList()); //burdaki bilgiler cookie'den doluyor..
        }
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            AppRole appRole = new AppRole
            {
                Name = roleViewModel.Name
            };
            IdentityResult result = _roleManager.CreateAsync(appRole).Result;
            if (result.Succeeded)
                return RedirectToAction("Roles");
            else AddModelError(result);
            return View(roleViewModel);
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
            }
            
            return RedirectToAction("Roles");
        }
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            RoleViewModel roleViewModel = role.Adapt<RoleViewModel>();
            return View(roleViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleViewModel roleViewModel)
        {
            var role = await _roleManager.FindByIdAsync(roleViewModel.Id);
            if (role != null)
            {
                role.Name = roleViewModel.Name;
                IdentityResult result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Roles");
                else
                    AddModelError(result);
            }
            return View(roleViewModel);
        }
        public async Task<IActionResult> RoleAssign(string id)
        {
            TempData["UserId"] = id;
            AppUser user = await _userManager.FindByIdAsync(id);
            IQueryable<AppRole> roles = _roleManager.Roles;
            ViewBag.userName = user.UserName;
            List<string> userRoles = await _userManager.GetRolesAsync(user) as List<string>;

            List<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>();

            foreach (var item in roles)
            {
                RoleAssignViewModel roleAssignViewModel = new RoleAssignViewModel();
                roleAssignViewModel.RoleId = item.Id;
                roleAssignViewModel.RoleName = item.Name;
                if (userRoles.Contains(item.Name))
                    roleAssignViewModel.Exist = true;
                else
                    roleAssignViewModel.Exist = false;
                roleAssignViewModels.Add(roleAssignViewModel);
            }
            return View(roleAssignViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels)
        {
            AppUser appUser = await _userManager.FindByIdAsync(TempData["UserId"].ToString());
            foreach (var item in roleAssignViewModels)
            {
                if (item.Exist)
                    await _userManager.AddToRoleAsync(appUser, item.RoleName);
                else
                    await _userManager.RemoveFromRoleAsync(appUser, item.RoleName);
            }
           
            return RedirectToAction("Users");
        }
        public IActionResult UsersChangePassword(string id)
        {
            AppUser appUser = _userManager.FindByIdAsync(id).Result;
            AdminChangePasswordViewModel adminChangePasswordViewModel = new AdminChangePasswordViewModel
            {
                UserId = appUser.Id,
                UserName = appUser.UserName
            };
            return View(adminChangePasswordViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UsersChangePassword(AdminChangePasswordViewModel adminChangePasswordViewModel)
        {
            AppUser appUser = await _userManager.FindByIdAsync(adminChangePasswordViewModel.UserId);
             string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            IdentityResult result = await _userManager.ResetPasswordAsync(appUser, token, adminChangePasswordViewModel.Password);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(appUser);
                return RedirectToAction("Users");
            }
            else
            {
                AddModelError(result);
            }
            return View(adminChangePasswordViewModel);
        }

    }
}
