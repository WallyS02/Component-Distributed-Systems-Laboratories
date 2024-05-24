using System;
using System.ServiceModel;

namespace _7
{

    public class HandlerZad6 : Zadanie7.IZadanie6Callback
    {
        public void Wynik(int wyn)
        {
            Console.WriteLine($"Wynik zwrotny: {wyn}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var client5 = new Zadanie7.Zadanie5Client();
            Console.WriteLine(client5.ScalNapisy(client5.ScalNapisy("zadanie", " dziala"), " dobrze"));
            Console.ReadKey();
            ((IDisposable)client5).Dispose();

            var client6 = new Zadanie7.Zadanie6Client(new InstanceContext(new HandlerZad6()));
            client6.Dodaj(1, 1);
            Console.ReadKey();
            ((IDisposable)client6).Dispose();
        }
    }
}
