
using CommandsService.EventProcessing.Interfaces;
using Helpers.RabbitMq;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;
using System.Threading.Channels;

namespace CommandsService.DataService.Async;

/// <summary>
/// This class is used to subscribe to a RabbitMQ message bus and process incoming messages.
/// The class has the following private fields:
/// _configuration: An IConfiguration object that contains the application's configuration settings.
/// _eventProcessor: An IEventProcessor object that processes incoming messages.
/// _connection: An IConnection object that represents the connection to the RabbitMQ server.
/// _channel: An IModel object that represents a channel on the RabbitMQ server.
/// _queueName: A string object that contains the name of the queue to which the subscriber is bound.
/// _rabbitMqHelper: A RabbitMQHelper object that provides helper methods for working with RabbitMQ.
/// </summary>
public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;
    private readonly string _rabbitMqHost, _rabbitMqPort = string.Empty;

    private RabbitMQHelper _rabbitMqHelper;


    public MessageBusSubscriber(
        IConfiguration configuration,
        IEventProcessor eventProcessor,
        RabbitMQHelper rabbitMQHelper
    )
    {
        _configuration = configuration;

        _rabbitMqHost = _configuration["RabbitMq:host"];

        _rabbitMqPort = _configuration["RabbitMq:prot"];

        _eventProcessor = eventProcessor;
        _rabbitMqHelper = rabbitMQHelper;

        InitializeRabbitMq();
    }

    /// <summary>
    /// Initializes the connection to the RabbitMQ server 
    /// and sets up the queue to which the subscriber is bound. 
    /// It also sets up an event handler for the ConnectionShutdown event 
    /// on the _connection object.
    /// </summary>
    private void InitializeRabbitMq()
    {
        try
        {
            Console.WriteLine($"--> Connecting to RabbitMQ with URL :{_configuration["RabbitMq:host"]}:{_configuration["RabbitMq:port"]}");
            // Setup Connection
            _connection = _rabbitMqHelper.GetConnection(
               host: _configuration["RabbitMq:host"],
               port: _configuration["RabbitMq:port"]
            );
            _channel = _rabbitMqHelper.channel;
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(
                queue: _queueName,
                exchange: "trigger",
                routingKey: ""
            );
            Console.WriteLine("--> Listening on the Message Bus ... ");

            _connection.ConnectionShutdown += _rabbitMqHelper.RabbitMQ_ConnectionShutdown;
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at MessageBusSubscriber > InittilizeRabbitMQ() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }


    /// <summary>
    /// This method is overridden from the BackgroundService base class. 
    /// This method is called when the service is started and runs asynchronously. 
    /// It sets up an event handler for the Received event on the _channel object. 
    /// When a message is received, the event handler is called, 
    /// and the message is processed by the _eventProcessor object. 
    /// The method then sets up the _channel object to consume messages 
    /// from the queue and returns a completed task.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task? task = null;
        try
        {
            stoppingToken.ThrowIfCancellationRequested();
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Executed");
                ReadOnlyMemory<byte> body = ea.Body;
                string notificationMessgae = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessgae);
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer
            );

            task = Task.CompletedTask;

        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at MessageBusSubscriber > InittilizeRabbitMQ() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return task;
    }
}