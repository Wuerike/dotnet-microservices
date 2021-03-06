using System.Text;
using CommandService.EventProcessing;
using CommandService.Settings;
using CommandService.Settings.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace CommandService.DataServices.Async
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly MessageBusVariables _variables;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IEnvironmentVariables environment, IEventProcessor eventProcessor)
        {
            _variables = environment.MessageBusVariables();
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory(){
                HostName = _variables.Host, 
                Port = int.Parse(_variables.Port)
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: _variables.Exchange, type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: _variables.Exchange, routingKey: "");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Log.Information("Connected and listening to the RabbitMQ message bus");
            }
            catch (Exception ex)
            {
                Log.Error($"Could not connect to the RabbitMQ message bus: {ex.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Log.Information("Event Received");
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Log.Information("RabbitMQ connection shutdown");
        }

        public override void Dispose()
        {
            Log.Information("RabbitMQ disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}