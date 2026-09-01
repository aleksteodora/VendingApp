using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace SecurityModule.BLL.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqPublisher(string hostName, int port)
        {
            var factory = new ConnectionFactory { HostName = hostName, Port = port };
            _connection = factory.CreateConnection();
        }

        public void Publish<T>(string queueName, T message)
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}