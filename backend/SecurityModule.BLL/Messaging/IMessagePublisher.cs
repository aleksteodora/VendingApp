namespace SecurityModule.BLL.Messaging
{
    public interface IMessagePublisher
    {
        void Publish<T>(string queueName, T message);
    }
}