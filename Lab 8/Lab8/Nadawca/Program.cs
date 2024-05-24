using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using Wiadomosci;

namespace Nadawca
{
    class Wiadomosc1 : IWiadomosc1
    {
        public string TrescWiadomosci1 { get; set; }
    }
    class Wiadomosc2 : IWiadomosc2
    {
        public string TrescWiadomosci2 { get; set; }
    }
    class Wiadomosc_1_2 : IWiadomosc_1_2
    {
        public string TrescWiadomosci1 { get; set; }
        public string TrescWiadomosci2 { get; set; }
    }

    internal class Program
    {
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
                for (int i = 0; i < 10; i++)
                {
                    bus.Publish(new Wiadomosc1() { TrescWiadomosci1 = $"Wiadomosc1 nr{i}" },
                        ctx =>
                        {
                            ctx.Headers.Set("student", $"Kutny {i}");
                            ctx.Headers.Set("indeks", $"188586 {i}");
                        });
                    bus.Publish(new Wiadomosc2() { TrescWiadomosci2 = $"Wiadomosc2 nr{i}" });
                    bus.Publish(new Wiadomosc_1_2() { TrescWiadomosci1 = $"Wiadomosc1 nr{i} v2", TrescWiadomosci2 = $"Wiadomosc2 nr{i} v2" },
                        ctx =>
                        {
                            ctx.Headers.Set("student", $"Kutny {i} v2");
                            ctx.Headers.Set("indeks", $"188586 {i} v2");
                        });
                }
            }

            bus.Stop();
        }
    }
}
