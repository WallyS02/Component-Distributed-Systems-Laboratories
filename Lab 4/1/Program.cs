using System;
using KSR_WCF1;
using System.ServiceModel;

namespace _1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fact = new ChannelFactory<IZadanie1>(new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost/ksr-wcf1-test"));
            var client = fact.CreateChannel();

            Console.WriteLine(client.Test("zadanie 1 dziala"));

            // Zad 5
            try
            {
                client.RzucWyjatek(true);
            }
            catch (FaultException<Wyjatek> e)
            {
                Console.WriteLine(client.OtoMagia(e.Detail.magia));//Test(e.Detail.opis));
            }

            // Zad 3
            var client2 = new ServiceReference1.Zadanie2Client();
            Console.WriteLine(client2.Test("zadanie 3 dziala"));

            // Zad 7
            var client7 = new ServiceReference2.Zadanie7Client();
            try
            {
                client7.RzucWyjatek7("a", 5);
            }
            catch (FaultException<Wyjatek> e)
            {
                Console.WriteLine("Wyjatek7");
            }

            ((IDisposable)client7).Dispose();
            ((IDisposable)client2).Dispose();
            ((IDisposable)client).Dispose();
            fact.Close();
            Console.ReadKey();
        }
    }
}