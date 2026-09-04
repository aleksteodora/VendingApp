namespace VendingManagement.BLL.Notifications
{
    public interface IWebhookNotifier
    {
        Task NotifyTransactionCompletedAsync(string? webhookUrl, Guid transactionPublicId, string status, string? token);
    }
}