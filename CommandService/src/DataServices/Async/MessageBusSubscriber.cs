using System.Text;
using CommandService.EventProcessing;
using CommandService.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.DataServices.Async
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IDictionary<string, string> _variables;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IEnvironmentVariables environment, IEventProcessor eventProcessor)
        {
            _variables = environment.MessageBus();
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory(){
                HostName = _variables["Host"], 
                Port = int.Parse(_variables["Port"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine($"--> Connected and listening to the RabbitMQ message bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the RabbitMQ message bus: {ex.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received");
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"--> RabbitMQ connection shutdown");
        }

        public override void Dispose()
        {
            Console.WriteLine($"--> RabbitMQ disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}