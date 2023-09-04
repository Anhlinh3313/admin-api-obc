using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Core.Infrastructure.Security
{
    public class AuthorizationRequirement : AuthorizationHandler<AuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
        {
            //context.User.Identity
            if(context.User.Identity.IsAuthenticated)
            {
                Console.WriteLine(context.User.Identity.Name);
                context.Succeed(requirement);
            }

            context.Fail();
            return Task.FromResult(0);
        }
    }
}