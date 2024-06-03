using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Helpers.RabbitMq;

/// <summary>
/// This class provides a way to connect to a 
/// RabbitMQ message broker and perform some basic operations.
/// The class has four properties : 
/// _connection: an instance of IConnection, which represents the connection to the RabbitMQ broker.
/// channel: an instance of IModel, which represents a channel on the connection.
/// _configuration: an instance of IConfiguration, which is used to configure the connection settings.
/// _factory: an instance of ConnectionFactory, which is used to create the connection.
/// </summary>
public class RabbitMQHelper
{
    private IConnection _connection { get; set; }
    public IModel channel { get; set; }
    private IConfiguration _configuration;
    private ConnectionFactory _factory { get; set; }

    public RabbitMQHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// This method sets up a connection to the RabbitMQ broker 
    /// and returns the connection object
    /// </summary>
    /// <returns></returns>
    public IConnection GetConnection(IConfiguration configuration)
    {
        try
        {
            _configuration = configuration;
            _factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMq:host"],
                Port = int.Parse(_configuration["RabbitMq:port"])
            };

            // Setup Connection
            _connection = _factory.CreateConnection();
            channel = _connection.CreateModel();

            // Declare exchange
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at RabbitMQHelper > InittilizeRabbitMQ() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return _connection;
    }

    public IConnection GetConnection(string host, string port)
    {
        try
        {
            _factory = new ConnectionFactory()
            {
                HostName = host,
                Port = int.Parse(port)
            };

            // Setup Connection
            _connection = _factory.CreateConnection();
            channel = _connection.CreateModel();

            // Declare exchange
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at RabbitMQHelper > InittilizeRabbitMQ() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return _connection;
    }

    /// <summary>
    /// This method is an event handler that is called when the RabbitMQ connection is shut down. 
    /// It simply logs a message to the console indicating that the connection has been shut down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        try
        {
            Console.WriteLine($"--> RabbitMQ Connection Shutdown");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at MessageBusClient > RabbitMQ_ConnectionShutdown() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    /// <summary>
    /// This method is used to dispose of the RabbitMQ connection and channel. 
    /// It:
    ///  1>Logs a message to the console indicating that the message bus is being disposed.
    ///  2> Checks if the channel is open and closes it if it is.
    ///  3> Closes the connection.
    /// </summary>
    public void Dispose()
    {
        try
        {
            Console.WriteLine("--> MessageBus Disposed");
            if (channel.IsOpen)
            {
                channel.Close();
                _connection.Close();
            }
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at MessageBusClient > Dispose() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}