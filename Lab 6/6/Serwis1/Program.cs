using System;
using System.ServiceModel;

namespace Serwis1
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
                "net.pipe://localhost/zadanie6-1");

            host.Open();
            Console.WriteLine("Serwis1");
            Console.ReadKey();
            host.Close();
        }
    }
}
