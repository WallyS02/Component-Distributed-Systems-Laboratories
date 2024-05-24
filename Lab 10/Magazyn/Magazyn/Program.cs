using Komunikaty;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Magazyn
{
    class Magazyn : IConsumer<IPytanieoWolne>, IConsumer<IAkceptacjaZamowienia>, IConsumer<IOdrzucenieZamowienia>
    {
        public int Wolne { get; set; } = 0;
        public int Zarezerwowane { get; set; } = 0;
        public Task Consume(ConsumeContext<IPytanieoWolne> context)
        {
            return Task.Run(() =>
            {

                if (Wolne >= context.Message.Ilosc)
                {
                    Wolne -= context.Message.Ilosc;
                    Zarezerwowane += context.Message.Ilosc;
                    Console.Out.WriteLineAsync($"Magazyn moza zrealizowac zamowienie {context.Message.CorrelationId} na ilosc {context.Message.Ilosc}");
                    context.RespondAsync(new OdpowiedzWolne() { CorrelationId = context.Message.CorrelationId });
                }
                else
                {
                    Console.Out.WriteLineAsync($"Magazyn nie ma wystarczajacej liczby produktow do zrealizowania zamowienia {context.Message.CorrelationId} na ilosc {context.Message.Ilosc}");
                    context.RespondAsync(new OdpowiedzWolneNegatywna() { CorrelationId = context.Message.CorrelationId });
                }
            });
        }

        public Task Consume(ConsumeContext<IAkceptacjaZamowienia> context)
        {
            return Task.Run(() =>
            {
                Zarezerwowane -= context.Message.Ilosc;
                Console.WriteLine($"Zrealizowano zamowienie: {context.Message.CorrelationId} na ilosc {context.Message.Ilosc} paczka wyslana do klienta");
            });
        }

        public Task Consume(ConsumeContext<IOdrzucenieZamowienia> context)
        {
            return Task.Run(() =>
            {
                Zarezerwowane -= context.Message.Ilosc;
                Wolne += context.Message.Ilosc;
                Console.WriteLine($"Odrzucono zamowienie: {context.Message.CorrelationId} na ilosc {context.Message.Ilosc} produkty zarezerwowane sa ponownie dostepne do sprzedazy");
            });
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var magazyn = new Magazyn();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("magazyn",
                    ep => ep.Instance(magazyn));
            });
            Console.WriteLine("Magazyn");
            bus.Start();

            int liczbaDostarczonychPorduktow = 0;

            while (true)
            {
                ConsoleKeyInfo opcja = Console.ReadKey(true);
                if (opcja.Key == ConsoleKey.D)
                {
                    Console.WriteLine("\nPodaj Liczbe Dostarczonych Produktow");
                    try
                    {
                        liczbaDostarczonychPorduktow = Convert.ToInt32(Console.ReadLine());
                        magazyn.Wolne += liczbaDostarczonychPorduktow;
                        Console.WriteLine($"\nStan Magazynu: \nDOSTAWA: {liczbaDostarczonychPorduktow} \nWolne: {magazyn.Wolne} \nZarezerwowane: {magazyn.Zarezerwowane}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Nie podales liczby! Anulowano operacje");
                    }
                }
                else if (opcja.Key == ConsoleKey.S)
                {
                    Console.WriteLine($"\nStan Magazynu: \nWolne: {magazyn.Wolne} \nZarezerwowane: {magazyn.Zarezerwowane}");
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
