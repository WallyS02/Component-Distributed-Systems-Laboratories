using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml;

namespace _5
{

    [ServiceContract]
    public interface IZadanie3
    {
        [OperationContract, WebGet(UriTemplate = "index.html"), XmlSerializerFormat]
        XmlDocument Serwuj();

        [OperationContract, WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
        int Dodaj(string a, string b);

        [OperationContract, WebGet(UriTemplate = "scripts.js")]
        Stream GetStream();
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var fabryka = new ChannelFactory<IZadanie3>(
                new WebHttpBinding(),
                new EndpointAddress("http://localhost:63545/Service1.svc/zadanie34"));
            fabryka.Endpoint.Behaviors.Add(new WebHttpBehavior());
            var klient = fabryka.CreateChannel();

            Console.WriteLine(klient.Dodaj("7", "6"));
            ((IDisposable)klient).Dispose();
            fabryka.Close();
            Console.ReadKey();
        }
    }
}
