using Mapster;
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

    }
}
