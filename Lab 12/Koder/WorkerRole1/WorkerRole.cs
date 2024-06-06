using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Użyj protokołu TLS 1.2 dla połączeń usługi Service Bus
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Ustaw maksymalną liczbę jednoczesnych połączeń
            ServicePointManager.DefaultConnectionLimit = 12;

            // Informacje dotyczące obsługi zmian konfiguracji
            // zawiera temat w witrynie MSDN pod adresem https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = GetQueueFromAzure("encodequeue");
            // TODO: Zastąp poniższy kod własną logiką.
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var msg = queue.GetMessage();
                if (msg != null)
                {
                    queue.DeleteMessage(msg);
                    string nazwa = msg.AsString;
                    var blobContainer = GetBlobFromAzure("files");

                    var blob = blobContainer.GetBlockBlobReference(nazwa);

                    if (blob == null)
                    {
                        continue;
                    }

                    var stream = new MemoryStream();
                    blob.DownloadToStream(stream);
                    string content = Encoding.UTF8.GetString(stream.ToArray());

                    string encodedString = "";
                    encodedString = EncodeROT13(content);

                    blob = blobContainer.GetBlockBlobReference(nazwa + "_encoded");

                    var bytes = new ASCIIEncoding().GetBytes(encodedString);
                    stream = new MemoryStream(bytes);
                    blob.UploadFromStream(stream);
                }
            }
        }

        private static string EncodeROT13(string input)
        {
            Random random = new Random();
            if (random.Next(3) == 0)
            {
                throw new Exception("Wyjatek kodowania");
            }

            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }

                array[i] = (char)number;
            }
            return new string(array);
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
