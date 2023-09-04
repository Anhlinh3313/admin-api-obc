using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Core;
using Core.Entity.Procedures;
using Core.Infrastructure.Helper.ExceptionHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.API
{
    public partial class Startup
    {
        private void MiddlewareConfig(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _delegate;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _delegate = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _delegate(httpContext);
            }
            catch (Exception ex)
            {
                await ResponseExceptionHelper.HandleExceptionAsync(httpContext, ex);
            }
        }
    }
}
