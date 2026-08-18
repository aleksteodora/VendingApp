using System.Text;
using System.Text.Json;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.WebApp.Services
{
    public class SecurityModuleClient : ISecurityModuleClient
    {
        // ka security modulu se nikad ne salje vise od jednog zahteva odjednom
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SecurityModuleClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> RequestTokenAsync(TokenRequestDataIn dataIn)
        {
            // sacekaj svoj red
            await _semaphore.WaitAsync();
            try
            {
                var baseUrl = _configuration["SecurityModule:BaseUrl"];
                var apiKey = _configuration["SecurityModule:ApiKey"];

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                var json = JsonSerializer.Serialize(dataIn);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/security-module/credit", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Security module returned error: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsed = JsonSerializer.Deserialize<ResponsePackage<string>>(responseBody, options);

                if (parsed == null || string.IsNullOrEmpty(parsed.Data))
                {
                    throw new InvalidOperationException("Security module did not return a valid token.");
                }

                return parsed.Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}