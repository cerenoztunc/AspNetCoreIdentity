using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC
{
    public class ExpiryDateRequirenment:IAuthorizationRequirement
    {
    }
    public class ExpiryDateHandler : AuthorizationHandler<ExpiryDateRequirenment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpiryDateRequirenment requirement)
        {
            if(context.User != null && context.User.Identity != null)
            {
                var claim = context.User.Claims.Where(x => x.Type == "ExpireDate" && x.Value != null).FirstOrDefault();
                if(claim != null)
                {
                    if(DateTime.Now > Convert.ToDateTime(claim.Value))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
