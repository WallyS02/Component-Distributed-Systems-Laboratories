using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;

namespace _3
{

    [ServiceContract]
    public interface IZadanie3
    {
        [OperationContract, WebGet(UriTemplate = "index.html"), XmlSerializerFormat]
        XmlDocument Serwuj();

        [OperationContract, WebGet(UriTemplate = "scripts.js")]
        Stream GetStream();

        [OperationContract, WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
        int Dodaj(string a, string b);
    }

    public class Service1 : IZadanie3
    {
        string indexFile = "C:\\Users\\Ja\\Downloads\\KSR\\Lab\\Lab 6\\index.xhtml";
        string scriptFile = "C:\\Users\\Ja\\Downloads\\KSR\\Lab\\Lab 6\\scripts.js";

        public XmlDocument Serwuj()
        {
            var xml = new XmlDocument();
            xml.Load(indexFile);
            return xml;
        }

        public int Dodaj(string a, string b)
        {
            return int.Parse(a) + int.Parse(b);
        }

        public Stream GetStream()
        {
            if (File.Exists(scriptFile))
            {
                return new FileStream(scriptFile, FileMode.Open);
            }
            return null;
        }
    }
}
