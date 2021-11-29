using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.CustomTagHelpers
{
    [HtmlTargetElement("td",Attributes = "user-roles")]
    public class UserRolesName:TagHelper
    {
        private readonly UserManager<AppUser> _userManager;
        public UserRolesName(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser user = await _userManager.FindByIdAsync(UserId);
            var roles = await _userManager.GetRolesAsync(user);
            string html = string.Empty;
            roles.ToList().ForEach(x =>
            {
                html += $"<span class='badge badge-info'>{x}</span>";
            });
            output.Content.SetHtmlContent(html);
        }
    }
}
