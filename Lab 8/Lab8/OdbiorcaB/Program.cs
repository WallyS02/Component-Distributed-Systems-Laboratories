using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaB
{
    class Odbiorca : IConsumer<IWiadomosc_1_2>
    {
        int licznik = 0;
        public Task Consume(ConsumeContext<IWiadomosc_1_2> ctx)
        {
            licznik += 1;
            return Console.Out.WriteLineAsync(
                $"Odebrano juz {licznik} wiadomosci: " +
                $"Info: student: {ctx.Headers.GetAll().Where(elem => elem.Key == "student").First().Value}" +
                $", indeks: {ctx.Headers.GetAll().Where(elem => elem.Key == "indeks").First().Value}" +
                $"\nTresc1: {ctx.Message.TrescWiadomosci1}" +
                $"\nTresc2: {ctx.Message.TrescWiadomosci2}");
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
