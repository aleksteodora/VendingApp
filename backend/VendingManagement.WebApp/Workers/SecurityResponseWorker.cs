using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.WebApp.Workers
{
    public class SecurityResponseWorker : BackgroundService
    {
        private const string QueueName = "security-module-responses";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SecurityResponseWorker> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public SecurityResponseWorker(IServiceProvider serviceProvider, ILogger<SecurityResponseWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("SecurityResponseWorker started, listening on queue '{QueueName}'.", QueueName);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<TokenResponseMessage>(json);

                if (message != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                    await transactionService.HandleTokenResponseAsync(message);
                }

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel!.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}