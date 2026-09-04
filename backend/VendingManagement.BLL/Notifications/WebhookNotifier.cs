using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VendingManagement.BLL.Notifications
{
    public class WebhookNotifier : IWebhookNotifier
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookNotifier> _logger;

        public WebhookNotifier(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<WebhookNotifier> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task NotifyTransactionCompletedAsync(string? webhookUrl, Guid transactionPublicId, string status, string? token)
        {
            if (string.IsNullOrEmpty(webhookUrl))
            {
               _logger.LogInformation("No webhook URL configured for transaction {TransactionId}, skipping notification.", transactionPublicId);
               return;
            }

            var payload = new
            {
                TransactionId = transactionPublicId,
                Status = status,
                Token = token,
                NotifiedAt = DateTime.UtcNow
            };

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Webhook notification sent successfully for transaction {TransactionId}.", transactionPublicId);
                }
                else
                {
                    _logger.LogWarning("Webhook notification failed for transaction {TransactionId}, status code: {StatusCode}.", transactionPublicId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending webhook notification for transaction {TransactionId}.", transactionPublicId);
            }
        }
    }
}