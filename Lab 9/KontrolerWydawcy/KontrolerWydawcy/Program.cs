using MassTransit;
using MassTransit.Serialization;
using System;
using Wiadomosci;

namespace KontrolerWydawcy
{
    class Publ : IPubl
    {
        public int numer { get; set; }
    }

    class Ustaw : IUstaw
    {
        public bool dziala { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.UseEncryptedSerializer(
                    new AesCryptoStreamProvider(
                    new Dostawca("18858618858618858618858618858618"),
                    "1885861885861885"));
            });
            bus.Start();
            Console.WriteLine("Kontroler nadawcy");
            Random random = new Random();
            while (true)
            {
                ConsoleKeyInfo opcja = Console.ReadKey(true);
                if (opcja.Key == ConsoleKey.S)
                {
                    bus.Publish(new Ustaw() { dziala = true }, ctx => {
                        ctx.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader,
                        Guid.NewGuid().ToString());
                    });
                }
                else if (opcja.Key == ConsoleKey.T)
                {
                    bus.Publish(new Ustaw() { dziala = false }, ctx => {
                        ctx.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader,
                        Guid.NewGuid().ToString());
                    });
                }
                else if (opcja.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            bus.Stop();
        }
    }
}
