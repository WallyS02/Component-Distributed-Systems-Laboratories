using System;
using System.ServiceModel;

namespace Klient
{

    [ServiceContract]
    public interface IZadanie6
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Klient");
            var fabryka = new ChannelFactory<IZadanie6>(
                new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost/Router"));
            var klient = fabryka.CreateChannel();
            Console.WriteLine(klient.Dodaj(160000, 5407));
            Console.ReadKey();
            ((IDisposable)klient).Dispose();
            fabryka.Close();
        }
    }
}
