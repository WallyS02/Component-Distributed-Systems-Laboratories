using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaA
{
    internal class Program
    {
        public static Task Handler(ConsumeContext<ITemperatura> ctx)
        {
            if (ctx.Headers.GetAll().Where(elem => elem.Key == "skala").First().Value.ToString() == "F" || ctx.Headers.GetAll().Where(elem => elem.Key == "skala").First().Value.ToString() == "F v2")
            {
                return Console.Out.WriteLineAsync(
                    $"Info: miejsce: {ctx.Headers.GetAll().Where(elem => elem.Key == "miejsce").First().Value}" +
                    $", skala: {ctx.Headers.GetAll().Where(elem => elem.Key == "skala").First().Value}" +
                    $"\nTemperatura w C: {5.0 / 9.0 * (ctx.Message.temperatura - 32.0)}");
            }
            else
            {
                return Console.Out.WriteLineAsync(
                    $"Info: miejsce: {ctx.Headers.GetAll().Where(elem => elem.Key == "miejsce").First().Value}" +
                    $", skala: {ctx.Headers.GetAll().Where(elem => elem.Key == "skala").First().Value}" +
                    $"\nTemperatura w F: {(9.0 / 5.0 * ctx.Message.temperatura) + 32.0}");
            }
        }


        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("recvqueueA", ep => {
                    ep.Handler<ITemperatura>(Handler);
                });
            });
            bus.Start();
            Console.WriteLine("Odbiorca A");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
