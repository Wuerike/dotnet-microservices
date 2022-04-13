using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Settings;
using RabbitMQ.Client;

namespace PlatformService.DataServices.Async
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange;

        public MessageBusClient(IEnvironmentVariables environment)
        {
            var variables = environment.MessageBusVariables();
            _exchange = variables["Exchange"];

            var factory = new ConnectionFactory(){
                HostName = variables["Host"], 
                Port = int.Parse(variables["Port"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine($"--> Connected to RabbitMQ message bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the RabbitMQ message bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishDto p)
        {
            var message = JsonSerializer.Serialize(p);

            if (_connection.IsOpen)
            {
                Console.WriteLine($"--> RabbitMQ connection is open, sending message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine($"--> RabbitMQ connection is closed, not sending message");
            }
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: _exchange, 
                routingKey: "", 
                basicProperties: null, 
                body: body
            );

            Console.WriteLine($"--> RabbitMQ have sent the message: {message}");
        }

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"--> RabbitMQ connection shutdown");
        }

        public void Dispose()
        {
            Console.WriteLine($"--> RabbitMQ disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}