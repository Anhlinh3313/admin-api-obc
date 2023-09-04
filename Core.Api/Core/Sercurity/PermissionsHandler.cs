using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.Infrastructure.Extensions;
using Core.Business.Services.Abstract;

namespace Core.Api.Core.Sercurity
{
	public class PermissionsHandler : AuthorizationHandler<PermissionsRequirement>
	{
		private readonly IPermissionService _iPermissionService;
		public PermissionsHandler(IPermissionService iPermissionService)
		{
			_iPermissionService = iPermissionService;
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
			if (context.User.Claims != null)
			{
				string controllerName = "", actionName = "";
				var nameIdentifier = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                var cHashType = context.User.Claims.FirstOrDefault(x => x.Type == "c_hash");
                var cHash = (cHashType != null) ? cHashType.Value : "";
                var userId = (nameIdentifier != null) ? nameIdentifier.Value : "";
				var mvcContext = context.Resource as AuthorizationFilterContext;
				var descriptor = mvcContext?.ActionDescriptor as ControllerActionDescriptor;

				if (descriptor != null)
				{
					controllerName = descriptor.ControllerName;
					actionName = descriptor.ActionName;
					//Create permission string based on the requested controller name and action name in the format 'controllername-action'
                    Console.WriteLine($"HandleRequirementAsync: userId: {userId}, controllerName: {controllerName}, actionName: {actionName}");
                    string requiredPermission = $"{controllerName}-{actionName}";
                    if (_iPermissionService.CheckUserPermission(userId.ToSafeInt(), requiredPermission))
					    context.Succeed(requirement);
					else
					    context.Fail();
				}
			}

			return Task.CompletedTask;
        }
    }
}
