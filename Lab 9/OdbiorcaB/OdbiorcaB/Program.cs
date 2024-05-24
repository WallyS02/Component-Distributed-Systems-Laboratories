using MassTransit;
using MassTransit.Serialization;
using System;
using System.Threading.Tasks;
using Wiadomosci;

namespace OdbiorcaB
{
    class OdpB : IOdpB
    {
        public string kto { get; set; }
    }

    internal class Program
    {
        public static Task Handler(ConsumeContext<IPubl> ctx)
        {
            if (ctx.Message.numer % 3 == 0)
            {
                ctx.RespondAsync(new OdpB() { kto = "abonent B" });
            }
            return Console.Out.WriteLineAsync($"Tresc wiadomosci: {ctx.Message.numer}");
        }

        public static Task HandleFault(ConsumeContext<Fault<IOdpB>> ctx)
        {
            foreach (var e in ctx.Message.Exceptions)
                Console.WriteLine(e.Message);
            return Console.Out.WriteLineAsync("Powyzej tresc wyjatku odpowiedzi dla odbiorcy B");
        }

        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.UseEncryptedSerializer(
                    new AesCryptoStreamProvider(
                    new Dostawca("18858618858618858618858618858618"),
                    "1885861885861885"));
                sbc.ReceiveEndpoint("publqueue", ep => {
                    ep.Handler<IPubl>(Handler);
                });
                sbc.ReceiveEndpoint("odpbqueue_error", ep => {
                    ep.Handler<Fault<IOdpB>>(HandleFault);
                });
            });
            bus.Start();
            Console.WriteLine("Odbiorca B");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
