using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace _2
{
    [ServiceContract]
    public interface IZadanie2
    {
        [OperationContract]
        string Test(string arg);
    }

    public class Zadanie2 : IZadanie2
    {
        public string Test(string arg)
        {
            return $"Testowanie argumentem: {arg}";
        }
    }

    [ServiceContract]
    public interface IZadanie7
    {
        [OperationContract]
        [FaultContract(typeof(Wyjatek7))]
        void RzucWyjatek7(string a, int b);
    }

    [DataContract]
    public class Wyjatek7
    {
        [DataMember]
        public string Opis { get; set; }

        [DataMember]
        public string A { get; set; }

        [DataMember]
        public int B { get; set; }
    }

    public class Zadanie7 : IZadanie7
    {
        public void RzucWyjatek7(string a, int b)
        {
            var exception = new FaultException<Wyjatek7>(new Wyjatek7(),
                new FaultReason("Wyjatek7 " + a + b));
            throw exception;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie2));
            host.AddServiceEndpoint(typeof(IZadanie2),
                new NetNamedPipeBinding(),
                "net.pipe://localhost/ksr-wcf1-zad2");

            var b = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(b);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                "net.pipe://localhost/metadane");

            // Zad 4
            host.AddServiceEndpoint(typeof(IZadanie2),
                new NetTcpBinding(),
                "net.tcp://127.0.0.1:55765");

            // Zad 7
            var host7 = new ServiceHost(typeof(Zadanie7));
            host7.AddServiceEndpoint(typeof(IZadanie7),
                new NetNamedPipeBinding(),
                "net.pipe://localhost/ksr-wcf1-zad7");

            var b7 = new ServiceMetadataBehavior();
            host7.Description.Behaviors.Add(b7);
            host7.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                "net.pipe://localhost/metadane7");

            host.Open();
            host7.Open();
            Console.ReadKey();
            host.Close();
            host7.Close();

            Console.ReadLine();
        }
    }
}