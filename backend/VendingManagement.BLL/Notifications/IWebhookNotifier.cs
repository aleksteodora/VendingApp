namespace VendingManagement.BLL.Notifications
{
    public interface IWebhookNotifier
    {
        Task NotifyTransactionCompletedAsync(Guid transactionPublicId, string status, string? token);
    }
}