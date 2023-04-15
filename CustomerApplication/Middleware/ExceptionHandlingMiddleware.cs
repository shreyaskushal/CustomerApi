using CustomerApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomerApplication.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";

				ErrorDetails errorDetails = new()
				{
					StatusCode = (int)HttpStatusCode.InternalServerError,
					Message = "An Internal Server Error has Occured"
				};

				string error = JsonSerializer.Serialize(errorDetails);
				await context.Response.WriteAsync(error);
			}
		}
	}
}
