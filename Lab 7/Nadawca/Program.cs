using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Nadawca
{
    class LabConsumer : DefaultBasicConsumer
    {
        public LabConsumer(IModel model) : base(model) { }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($"{message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("nadawca");
            Console.WriteLine("nacisnij aby wyslac");
            Console.ReadKey();

            string queueName = "kolejka188586";
            string url = "amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc";
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(url)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);
                string replyQueueName = channel.QueueDeclare().QueueName;
                var consumer = new LabConsumer(channel);
                channel.BasicConsume(replyQueueName, true, consumer);

                for (int i = 0; i < 10; i++)
                {
                    var body = Encoding.UTF8.GetBytes($"wiadomosc{i + 1}");
                    IBasicProperties properties = channel.CreateBasicProperties();

                    properties.ReplyTo = replyQueueName;
                    var corrId = Guid.NewGuid().ToString();
                    properties.CorrelationId = corrId;

                    properties.Headers = new Dictionary<string, object>
                    {
                        { "naglowek1", Encoding.UTF8.GetBytes($"tresc1 + nr {i + 1}") },
                        { "naglowek2", Encoding.UTF8.GetBytes($"tresc2 + nr {i + 1}") }
                    };

                    channel.BasicPublish("", queueName, properties, body);
                    System.Threading.Thread.Sleep(2000);
                }
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("wyslano wiadomosci");
            }

            Console.WriteLine("zadanie7 nacisnij aby wyslac");
            Console.ReadKey();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("abc", "topic");

                for (int i = 0; i < 10; i++)
                {
                    var body = Encoding.UTF8.GetBytes($"message{i + 1} zadanie7");

                    if (i % 2 == 0)
                    {
                        channel.BasicPublish("abc", "abc.def", null, body);
                    }
                    else
                    {
                        channel.BasicPublish("abc", "abc.xyz", null, body);
                    }
                }

                Console.ReadKey();
            }
        }
    }
}
