using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VendingManagement.DAL.UOW.Interfaces;

namespace VendingManagement.WebApp.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string HeaderName = "x-api-key";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                context.Result = new JsonResult(new { message = "Unauthorized - missing API key." })
                { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            var unitOfWork = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWork)) as IUnitOfWork;

            var userExists = await unitOfWork!.CustomerRepository.ExistsByApiKeyAsync(extractedApiKey.ToString());

            if (!userExists)
            {
                context.Result = new JsonResult(new { message = "Unauthorized - invalid API key." })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}