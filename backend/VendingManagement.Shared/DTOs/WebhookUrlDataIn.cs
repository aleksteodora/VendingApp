using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.DTOs
{
    public class WebhookUrlDataIn
    {
        [Required(ErrorMessage = "Webhook URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string WebhookUrl { get; set; }
    }
}