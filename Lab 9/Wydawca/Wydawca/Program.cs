using MassTransit;
using MassTransit.Serialization;
using System;
using System.Threading.Tasks;
using Wiadomosci;

namespace Wydawca
{
    class Publ : IPubl
    {
        public int numer {  get; set; }
    }

    class ConsumerObserver : IConsumeObserver
    {
        public int liczbaProbObsluzenia;
        public int liczbaPomyslnieObsluzonych;
        public int liczbaNiepomyslnieObsluzonych;

        public ConsumerObserver() {
            liczbaProbObsluzenia = 0;
            liczbaPomyslnieObsluzonych = 0;
            liczbaNiepomyslnieObsluzonych = 0;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            liczbaNiepomyslnieObsluzonych += 1;
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            liczbaPomyslnieObsluzonych += 1;
            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            liczbaProbObsluzenia += 1;
            return Task.CompletedTask;
        }
    }

    class PublishObserver : IPublishObserver
    {
        public int liczbaOpublikowanych;

        public PublishObserver()
        {
            liczbaOpublikowanych = 0;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            liczbaOpublikowanych += 1;
            return Task.CompletedTask;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }

    internal class Program
    {
        static int numer = 0;
        static bool ustaw = false;
        static int liczbaPotworzen = 5;

        public static Task Handler(ConsumeContext<IUstaw> ctx)
        {
            ustaw = ctx.Message.dziala;
            return Console.Out.WriteLineAsync($"Zmieniono dzialanie na: {ctx.Message.dziala}");
        }

        public static Task Handler(ConsumeContext<IOdpA> ctx)
        {
            if (new Random().Next(100) < 33)
            {
                throw new Exception("Wyjatek A");
            }
            return Console.Out.WriteLineAsync($"Odpowiedz od: {ctx.Message.kto}");
        }

        public static Task Handler(ConsumeContext<IOdpB> ctx)
        {
            if (new Random().Next(100) < 33)
            {
                throw new Exception("Wyjatek B");
            }
            return Console.Out.WriteLineAsync($"Odpowiedz od: {ctx.Message.kto}");
        }

        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.UseEncryptedSerializer(
                    new AesCryptoStreamProvider(
                    new Dostawca("18858618858618858618858618858618"),
                    "1885861885861885"));
                sbc.ReceiveEndpoint("kontroler", ep => {
                    ep.Handler<IUstaw>(Handler);
                });
                sbc.ReceiveEndpoint("odpaqueue", ep => {
                    ep.Handler<IOdpA>(Handler);
                    ep.UseMessageRetry(r => { r.Immediate(liczbaPotworzen); });
                });
                sbc.ReceiveEndpoint("odpbqueue", ep => {
                    ep.Handler<IOdpB>(Handler);
                    ep.UseMessageRetry(r => { r.Immediate(liczbaPotworzen); });
                });
            });
            ConsumerObserver consumerObserver = new ConsumerObserver();
            PublishObserver publishObserver = new PublishObserver();
            bus.ConnectConsumeObserver(consumerObserver);
            bus.ConnectPublishObserver(publishObserver);
            bus.Start();
            Console.WriteLine("Nadawca");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo opcja = Console.ReadKey(true);
                    if (opcja.Key == ConsoleKey.S)
                    {
                        Console.WriteLine($"Liczba prób obsłużenia komunikatów każdego typu: {consumerObserver.liczbaProbObsluzenia}");
                        Console.WriteLine($"Liczba pomyślnie obsłużonych komunikatów każdego typu: {consumerObserver.liczbaPomyslnieObsluzonych}");
                        Console.WriteLine($"Liczba niepomyślnie obsłużonych komunikatów każdego typu: {consumerObserver.liczbaNiepomyslnieObsluzonych}");
                        Console.WriteLine($"Liczba opublikowanych komunikatów każdego typu: {publishObserver.liczbaOpublikowanych}");
                    }
                    else
                    {
                        break;
                    }
                }
                else if (ustaw)
                {
                    bus.Publish(new Publ() { numer = numer });
                    numer++;
                    System.Threading.Thread.Sleep(1000);
                }
            }
            bus.Stop();
        }
    }
}
