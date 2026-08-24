using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using VendingManagement.DAL.UOW.Interfaces;

namespace VendingManagement.WebApp.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string HeaderName = "x-api-key";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetService(typeof(ILogger<ApiKeyAuthAttribute>)) as ILogger<ApiKeyAuthAttribute>;

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                logger?.LogWarning("Authentication failed: missing API key. Path={Path}", context.HttpContext.Request.Path);

                context.Result = new JsonResult(new { message = "Unauthorized - missing API key." })
                { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            var unitOfWork = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWork)) as IUnitOfWork;

            var userExists = await unitOfWork!.CustomerRepository.ExistsByApiKeyAsync(extractedApiKey.ToString());

            if (!userExists)
            {
                logger?.LogWarning("Authentication failed: invalid API key. Path={Path}", context.HttpContext.Request.Path);

                context.Result = new JsonResult(new { message = "Unauthorized - invalid API key." })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}