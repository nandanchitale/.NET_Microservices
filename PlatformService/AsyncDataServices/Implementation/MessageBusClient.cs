using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices.Interfaces;
using PlatformService.DTO;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices.Implementation;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = _configuration.GetSection("RabbitMq").GetSection("host").Value,
            Port = int.Parse(_configuration.GetSection("RabbitMq").GetSection("port").Value)
        };

        try
        {
            // Setup Connection
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected to RabbitMQ Bus");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("--> Could not Connect to message bus");
            Console.WriteLine($"Exception at MessageBusClient > Constructor() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        try
        {
            string message = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is Open, Sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Conneciton is closed, not sending messgae");
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at MessageBusClient > PublishNewPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        try
        {
            Console.WriteLine($"--> RabbitMQ Connection Shutdown");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at MessageBusClient > RabbitMQ_ConnectionShutdown() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    private void SendMessage(string message)
    {
        try
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"--> We have sent {message}");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at MessageBusClient > SendMessage() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public void Dispose()
    {
        try
        {
            Console.WriteLine("--> MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at MessageBusClient > Dispose() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
