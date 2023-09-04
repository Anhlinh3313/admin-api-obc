using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Core.Infrastructure.Helper.ExceptionHelper
{
	public static class ResponseExceptionHelper
	{
		public static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			if (exception == null) return;

			var code = HttpStatusCode.InternalServerError;

			if (exception is KeyNotFoundException) code = HttpStatusCode.NotFound;
			else if (exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
			else if (exception is Exception) code = HttpStatusCode.BadRequest;

			await WriteExceptionAsync(context, exception, code).ConfigureAwait(false);
		}

		private static async Task WriteExceptionAsync(HttpContext context, Exception exception, HttpStatusCode code)
		{
			var response = context.Response;
			await response.WriteAsync(JsonConvert.SerializeObject(new
			{
				isSuccess = false,
				exception = new
				{
					type = exception.GetType().Name,
					message = exception.Message,
					strackTrace = exception.StackTrace
				}
			})).ConfigureAwait(false);
		}
	}
}
