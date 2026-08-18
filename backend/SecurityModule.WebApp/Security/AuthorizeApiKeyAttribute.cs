using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecurityModule.WebApp.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeApiKeyAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _headerName;

        public AuthorizeApiKeyAttribute(IConfiguration configuration,
            string apiKey,
            string headerName)
        {
            _configuration = configuration;
            _headerName = headerName;
            _apiKey = apiKey;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_headerName, out var extractedApiKey))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            string? actualKey = _configuration.GetSection(_apiKey).Value;
            if (string.IsNullOrEmpty(actualKey) || !actualKey.Equals(extractedApiKey))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
