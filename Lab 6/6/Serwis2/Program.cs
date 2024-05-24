using System;
using System.ServiceModel;

namespace Serwis2
{

    [ServiceContract]
    public interface IZadanie6
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }
    public class Zadanie6 : IZadanie6
    {
        public int Dodaj(int a, int b)
        {
            return a + b;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie6));
            host.AddServiceEndpoint(
                typeof(IZadanie6),
                new NetNamedPipeBinding(),
                "net.pipe://localhost/zadanie6-2");

            host.Open();
            Console.WriteLine("Serwis2");
            Console.ReadKey();
            host.Close();
        }
    }
}
