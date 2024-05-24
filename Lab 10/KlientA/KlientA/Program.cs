using Komunikaty;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace KlientA
{
    static class DaneKlienta
    {
        public const string Login = "KlientA";
    }

    class Skrzynka : IConsumer<IPytanieoPotwierdzenie>, IConsumer<IAkceptacjaZamowienia>, IConsumer<IOdrzucenieZamowienia>
    {
        public Task Consume(ConsumeContext<IPytanieoPotwierdzenie> context)
        {
            if (context.Message.Login == DaneKlienta.Login)
            {
                Console.WriteLine($"Czy zaakceptowac zamowienie {context.Message.CorrelationId}: ");
                bool odp = false;
                ConsoleKeyInfo opcja = Console.ReadKey(true);
                if (opcja.Key == ConsoleKey.T)
                    odp = true;
                else if (opcja.Key == ConsoleKey.N)
                    odp = false;
                Console.WriteLine();
                return Task.Run(() =>
                {
                    if (odp)
                        context.RespondAsync(new Potwierdzenie() { CorrelationId = context.Message.CorrelationId });
                    else
                        context.RespondAsync(new BrakPotwierdzenia() { CorrelationId = context.Message.CorrelationId });
                });
            }
            else
                return Task.Run(() => { });
        }

        public Task Consume(ConsumeContext<IAkceptacjaZamowienia> context)
        {
            if (context.Message.Login == DaneKlienta.Login)
                return Console.Out.WriteLineAsync($"Zaakceptowano zamowienie {context.Message.CorrelationId} na ilosc {context.Message.Ilosc}");
            else
                return Task.Run(() => { });
        }

        public Task Consume(ConsumeContext<IOdrzucenieZamowienia> context)
        {
            if (context.Message.Login == DaneKlienta.Login)
                return Console.Out.WriteLineAsync($"Odrzucono zamowienie {context.Message.CorrelationId} na ilosc {context.Message.Ilosc}");
            else
                return Task.Run(() => { });
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("klienta", ep =>
                {
                    var skrzynka = new Skrzynka();
                    ep.Instance(skrzynka);
                });
            });
            bus.Start();
            Console.WriteLine(DaneKlienta.Login);
            while (true)
            {
                ConsoleKeyInfo opcja = Console.ReadKey(true);
                if (opcja.Key == ConsoleKey.Z)
                {
                    Console.WriteLine("\nPodaj Ilosc");

                    int ilosc = 0;
                    try
                    {
                        ilosc = Convert.ToInt32(Console.ReadLine());
                        bus.Publish(new StartZamowienia() { Login = DaneKlienta.Login, Ilosc = ilosc });
                        Console.WriteLine();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Podaj poprawna liczbe");
                    }
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
