using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Security.Principal;

namespace WCFServiceWebRole1
{
    // UWAGA: możesz użyć polecenia „Zmień nazwę” w menu „Refaktoryzuj”, aby zmienić nazwę klasy „Service1” w kodzie, usłudze i pliku konfiguracji.
    // UWAGA: aby uruchomić klienta testowego WCF w celu przetestowania tej usługi, wybierz plik Service1.svc lub Service1.svc.cs w eksploratorze rozwiązań i rozpocznij debugowanie.
    public class Service1 : IService1
    {
        public bool Koduj(string nazwa, string tresc)
        {
            var blobContainer = GetBlobFromAzure("files");

            var blob = blobContainer.GetBlockBlobReference(nazwa);

            var bytes = new ASCIIEncoding().GetBytes(tresc);
            var stream = new MemoryStream(bytes);
            blob.UploadFromStream(stream);

            var queue = GetQueueFromAzure("encodequeue");
            var msg = new CloudQueueMessage(nazwa);
            queue.AddMessage(msg);

            return true;
        }

        public string Pobierz(string nazwa)
        {
            var blobContainer = GetBlobFromAzure("files");

            var blob = blobContainer.GetBlockBlobReference(nazwa + "_encoded");

            if (blob == null)
            {
                return string.Empty;
            }

            var stream = new MemoryStream();
            blob.DownloadToStream(stream);
            string content = Encoding.UTF8.GetString(stream.ToArray());

            return content;
        }

        private static CloudBlobContainer GetBlobFromAzure(string nameOfBlobContainer)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(nameOfBlobContainer);
            container.CreateIfNotExists();
            return container;
        }

        private static CloudQueue GetQueueFromAzure(string nameOfQueue)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(nameOfQueue);
            queue.CreateIfNotExists();
            return queue;
        }
    }
}
