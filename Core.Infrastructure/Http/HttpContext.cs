using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Core.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Http
{
    public static class HttpContext
    {
        private static IHttpContextAccessor _contextAccessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor.HttpContext;

        public static int CurrentUserId 
        { 
            get
            {
				var nameIdentifier = Current.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
				var userId = (nameIdentifier != null) ? nameIdentifier.Value : "";
				return userId.ToSafeInt();
            }
        }

        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        } 
    }
}
