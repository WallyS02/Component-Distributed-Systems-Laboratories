using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaA
{
    internal class Program
    {
        public static Task Handler(ConsumeContext<IWiadomosc1> ctx) =>
               Console.Out.WriteLineAsync(
                   $"Info: student: {ctx.Headers.GetAll().Where(elem => elem.Key == "student").First().Value}" +
                   $", indeks: {ctx.Headers.GetAll().Where(elem => elem.Key == "indeks").First().Value}" +
                   $"\nTresc: {ctx.Message.TrescWiadomosci1}");


        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("recvqueueA", ep => {
                    ep.Handler<IWiadomosc1>(Handler);
                });
            });
            bus.Start();
            Console.WriteLine("Odbiorca A");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
