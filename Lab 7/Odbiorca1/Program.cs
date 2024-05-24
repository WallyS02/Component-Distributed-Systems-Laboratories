using RabbitMQ.Client;
using System;
using System.Text;

namespace Odbiorca1
{
    class LabConsumer : DefaultBasicConsumer
    {
        public LabConsumer(IModel model) : base(model) { }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());

            if (properties.Headers != null)
            {
                string naglowek1 = Encoding.UTF8.GetString((byte[])properties.Headers["naglowek1"]);
                string naglowek2 = Encoding.UTF8.GetString((byte[])properties.Headers["naglowek2"]);
                Console.WriteLine($"{naglowek1} {naglowek2} {message}");
            }
            else
            {
                Console.WriteLine(message);
            }
            System.Threading.Thread.Sleep(2000);
            Model.BasicAck(deliveryTag, false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("odbiorca1");

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
                var consumer = new LabConsumer(channel);
                channel.BasicQos(0, 1, false);
                channel.BasicConsume(queueName, false, consumer);
                Console.ReadKey();
            }

            Console.WriteLine("zadanie7");
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("abc", "topic");
                var queueName6 = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName6, "abc", "abc.#");


                var consumer = new LabConsumer(channel);
                channel.BasicConsume(queueName6, false, consumer);

                Console.ReadKey();

            }
        }
    }
}
