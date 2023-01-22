using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Queue.PublishSubscriber.Subscriber;

public class Receive
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("logs", ExchangeType.Fanout); //create a fanout exchange

            var queueName = channel.QueueDeclare(queue:"queue.log.1", exclusive: false).QueueName; // non-durable, exclusive, autodelete 
            
            //if exlusice = true , cannot scale horizontaly. Two instaces gets exception


            channel.QueueBind(queue: queueName,
                          exchange: "logs",
                          routingKey: "");

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue:  queueName,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}