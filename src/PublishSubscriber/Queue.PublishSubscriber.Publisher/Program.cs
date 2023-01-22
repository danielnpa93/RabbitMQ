using RabbitMQ.Client;
using System.Text;

namespace Queue.PublishSubscriber.Publisher;

public class Send
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("logs", ExchangeType.Fanout); //create a fanout exchange

            var message = GetMessage(args);

            var body = Encoding.UTF8.GetBytes(message);

            //var properties = channel.CreateBasicProperties();
            //properties.Persistent = true;// if persisted message, depends on publishser.  Queue durable also requirene

            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,// null is default
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);

        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }

}