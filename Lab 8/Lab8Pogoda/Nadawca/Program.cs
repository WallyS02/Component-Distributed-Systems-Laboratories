using MassTransit;
using System;
using Wiadomosci;

namespace Nadawca
{
    class Temperatura : ITemperatura
    {
        public double temperatura { get; set; }
    }
    class Cisnienie : ICisnienie
    {
        public double cisnienie { get; set; }
    }
    class TempICisn : ITempICisn
    {
        public double temperatura { get; set; }
        public double cisnienie { get; set; }
    }

    internal class Program
    {
        static string[] miejsca = { "Gdansk", "Gdynia", "Sopot" };
        static string[] skale = { "C", "F" };
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
            });
            bus.Start();
            Console.WriteLine("Nadawca");

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Random random = new Random();
                for (int i = 0; i < 10; i++)
                {
                    string miejsce = miejsca[random.Next(miejsca.Length)];
                    string skala = skale[random.Next(skale.Length)];
                    double temperatura;
                    if (skala == "F")
                    {
                        temperatura = random.NextDouble() * 500;
                    }
                    else
                    {
                        temperatura = random.NextDouble() * 50;
                    }
                    bus.Publish(new Temperatura() { temperatura = temperatura },
                        ctx =>
                        {
                            ctx.Headers.Set("miejsce", miejsce);
                            ctx.Headers.Set("skala", skala);
                        });
                    bus.Publish(new Cisnienie() { cisnienie = random.NextDouble() * 1000 });
                    miejsce = miejsca[random.Next(miejsca.Length)];
                    skala = skale[random.Next(skale.Length)];
                    if (skala == "F")
                    {
                        temperatura = random.NextDouble() * 500;
                    }
                    else
                    {
                        temperatura = random.NextDouble() * 50;
                    }
                    bus.Publish(new TempICisn() { temperatura = temperatura, cisnienie = random.NextDouble() * 1000 },
                        ctx =>
                        {
                            ctx.Headers.Set("miejsce", miejsce + " v2");
                            ctx.Headers.Set("skala", skala + " v2");
                        });
                }
            }

            bus.Stop();
        }
    }
}
