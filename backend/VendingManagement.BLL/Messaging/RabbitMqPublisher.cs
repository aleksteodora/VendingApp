using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace VendingManagement.BLL.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly string _hostName;

        public RabbitMqPublisher(string hostName)
        {
            _hostName = hostName;
        }

        public void Publish<T>(string queueName, T message)
        {
            var factory = new ConnectionFactory { HostName = _hostName };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: true, //prezivljava restart servera, cuvanje na disk
                exclusive: false, //vise servisa moye istovremeno slusati
                autoDelete: false, //red se ne brise sam iako nema aktivnih slusalaca
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
    }
}