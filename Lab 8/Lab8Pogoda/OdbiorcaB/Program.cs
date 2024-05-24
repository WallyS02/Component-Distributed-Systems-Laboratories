using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaB
{
    class Odbiorca : IConsumer<ITempICisn>
    {
        int licznik = 0;
        public Task Consume(ConsumeContext<ITempICisn> ctx)
        {
            licznik += 1;
            return Console.Out.WriteLineAsync(
                $"Odebrano juz {licznik} wiadomosci: " +
                $"Info: miejsce: {ctx.Headers.GetAll().Where(elem => elem.Key == "miejsce").First().Value}" +
                $", skala: {ctx.Headers.GetAll().Where(elem => elem.Key == "skala").First().Value}" +
                $"\nTemperatura: {ctx.Message.temperatura}" +
                $"\nCisnienie: {ctx.Message.cisnienie}");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("recvqueueB", ep =>
                {
                    var odbiorca = new Odbiorca();
                    ep.Instance(odbiorca);
                });
            });
            bus.Start();
            Console.WriteLine("Odbiorca B");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
