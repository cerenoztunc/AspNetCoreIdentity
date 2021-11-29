using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.MVC.ClaimProviders
{
    public class ClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if(principal!=null && principal.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = principal.Identity as ClaimsIdentity;
                AppUser appUser = await _userManager.FindByNameAsync(claimsIdentity.Name);
                if(appUser != null)
                {
                    if(appUser.BirthDay != null)
                    {
                        var userAge = DateTime.Today.Year - appUser.BirthDay?.Year;
                        if(userAge > 15)
                        {
                            Claim violanceClaim = new Claim("Violance", true.ToString(), ClaimValueTypes.String, "Internal");
                            claimsIdentity.AddClaim(violanceClaim);
                            
                        }
                        
                    }
                    if(appUser.City != null)
                    {
                        if (!principal.HasClaim(c => c.Type == "city"))
                        {
                            Claim cityClaim = new Claim("city", appUser.City, ClaimValueTypes.String, "Internal");
                            claimsIdentity.AddClaim(cityClaim);
                        }
                    }
                }
            }
            return principal;
        }
    }
}
