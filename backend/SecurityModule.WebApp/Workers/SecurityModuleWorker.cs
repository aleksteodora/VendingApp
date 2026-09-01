using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecurityModule.BLL.Messaging;
using SecurityModule.BLL.Services.Interfaces;
using VendingManagement.Shared.DTOs;

namespace SecurityModule.WebApp.Workers
{
    public class SecurityModuleWorker : BackgroundService
    {
        private const string RequestQueue = "security-module-requests";
        private const string ResponseQueue = "security-module-responses";

        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SecurityModuleWorker> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public SecurityModuleWorker(IServiceProvider serviceProvider, IMessagePublisher messagePublisher, IConfiguration configuration, ILogger<SecurityModuleWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
            _configuration = configuration;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var host = _configuration["RabbitMQ:Host"];
            var port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672");

            var factory = new ConnectionFactory { HostName = host, Port = port };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: RequestQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: ResponseQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _logger.LogInformation("SecurityModuleWorker started, listening on queue '{QueueName}'.", RequestQueue);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<TokenRequestMessage>(json);

                if (message != null)
                {
                    ProcessMessage(message);
                }

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel!.BasicConsume(queue: RequestQueue, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void ProcessMessage(TokenRequestMessage message)
        {
            //Thread.Sleep(6000);

            using var scope = _serviceProvider.CreateScope();
            var securityModuleService = scope.ServiceProvider.GetRequiredService<ISecurityModuleService>();

            TokenResponseMessage response;

            try
            {
                var dataIn = new TokenRequestDataIn
                {
                    MeterSerialNumber = message.MeterSerialNumber,
                    Amount = message.Amount
                };

                var result = securityModuleService.GenerateRandomToken(dataIn);

                response = new TokenResponseMessage
                {
                    TransactionId = message.TransactionId,
                    Token = result.Data,
                    Success = true
                };

                _logger.LogInformation("Token generated for transaction {TransactionId}.", message.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate token for transaction {TransactionId}.", message.TransactionId);

                response = new TokenResponseMessage
                {
                    TransactionId = message.TransactionId,
                    Token = string.Empty,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }

            _messagePublisher.Publish(ResponseQueue, response);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
