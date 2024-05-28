using Helpers.RabbitMq;
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
    private RabbitMQHelper _rabbitMqHelper;
    private readonly string _rabbitMqHost, _rabbitMqPort = string.Empty;
    public MessageBusClient(
        IConfiguration configuration,
        RabbitMQHelper rabbitMQHelper
    )
    {
        _configuration = configuration;

        _rabbitMqHost = _configuration["RabbitMq:host"];

        _rabbitMqPort = _configuration["RabbitMq:prot"];

        _rabbitMqHelper = rabbitMQHelper;

        try
        {
            // Setup Connection
            _connection = _rabbitMqHelper.GetConnection(_configuration);
            _channel = _rabbitMqHelper.channel;

            // Declare exchange
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += _rabbitMqHelper.RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected to RabbitMQ Bus");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("--> Could not Connect to message bus");
            Console.WriteLine($"--> Exception at MessageBusClient > Constructor() => {ex.Message}");
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
            Console.WriteLine($"--> Exception at MessageBusClient > PublishNewPlatform() => {ex.Message}");
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
            Console.WriteLine($"--> Exception at MessageBusClient > SendMessage() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
