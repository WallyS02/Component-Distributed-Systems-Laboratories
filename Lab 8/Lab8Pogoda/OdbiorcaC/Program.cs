using MassTransit;
using System;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaC
{
    internal class Program
    {
        public static Task Handler(ConsumeContext<ICisnienie> ctx) =>
            Console.Out.WriteLineAsync($"\nCisnienie: {ctx.Message.cisnienie}");

        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("recvqueueC", ep =>
                {
                    ep.Handler<ICisnienie>(Handler);
                });
            });
            bus.Start();
            Console.WriteLine("Odbiorca C");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
