using Komunikaty;
using MassTransit;
using System;

namespace Sklep
{
    public class RejestracjaZamowienie : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string Login { get; set; }
        public int Ilosc { get; set; }
        public Guid? TimeoutId { get; set; }
    }
    public class RejestracjaSklep : MassTransitStateMachine<RejestracjaZamowienie>
    {
        public State Niepotwierdzone { get; private set; }
        public State PotwierdzoneKlient { get; private set; }
        public State PotwierdzoneMagazyn { get; private set; }

        public Event<StartZamowienia> StartZamowienia { get; private set; }
        public Event<Potwierdzenie> Potwierdzenie { get; private set; }
        public Event<BrakPotwierdzenia> BrakPotwierdzenia { get; private set; }
        public Event<OdpowiedzWolne> OdpowiedzWolne { get; private set; }
        public Event<OdpowiedzWolneNegatywna> OdpowiedzWolneNegatywna { get; set; }
        public Event<Timeout> TimeoutEvent { get; private set; }
        public Schedule<RejestracjaZamowienie, Timeout> TO { get; set; }

        public RejestracjaSklep()
        {
            InstanceState(x => x.CurrentState);

            Event(() => StartZamowienia,
                x => x.CorrelateBy(
                        s => s.Login,
                        ctx => ctx.Message.Login
                    ).SelectId(context => Guid.NewGuid())
                );

            Schedule(() => TO,
                    x => x.TimeoutId,
                    x => { x.Delay = TimeSpan.FromSeconds(10); }
                );

            Initially(

                When(StartZamowienia)
                .Schedule(TO, ctx => new Timeout() { CorrelationId = ctx.Saga.CorrelationId })
                .Then(ctx => ctx.Saga.Login = ctx.Message.Login)
                .Then(ctx => ctx.Saga.Ilosc = ctx.Message.Ilosc)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"zamowienie dla {ctx.Message.Login} na w ilosci {ctx.Message.Ilosc}"); })
                .Respond(ctx => { return new PytanieoPotwierdzenie() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login }; })
                .Respond(ctx => { return new PytanieoWolne() { CorrelationId = ctx.Saga.CorrelationId, Ilosc = ctx.Saga.Ilosc }; })
                .TransitionTo(Niepotwierdzone)
                );

            During(Niepotwierdzone,

                When(TimeoutEvent)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"TIMEOUT: dla uzytkownika {ctx.Saga.Login} na zamowienie {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize(),

                When(Potwierdzenie)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"{ctx.Saga.Login} potwierdzil zamowienie {ctx.Message.CorrelationId}"); })
                .Unschedule(TO)
                .TransitionTo(PotwierdzoneKlient),

                When(BrakPotwierdzenia)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"{ctx.Saga.Login} nie potwierdzil zamowienia {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize(),

                When(OdpowiedzWolne)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"Magazyn moze zrealizowac zamowienie {ctx.Message.CorrelationId}"); })
                .TransitionTo(PotwierdzoneMagazyn),

                When(OdpowiedzWolneNegatywna)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"Magazyn nie moze zrealizowac zamowienia {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize()
                );

            During(PotwierdzoneKlient,

                When(OdpowiedzWolne)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"Magazyn moze zrealizowac zamowienie {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new AkceptacjaZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize(),

                When(OdpowiedzWolneNegatywna)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"Magazyn nie moze zrealizowac zamowienia {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize()
                );

            During(PotwierdzoneMagazyn,

                When(TimeoutEvent)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"TIMEOUT: dla uzytkownika {ctx.Saga.Login} na zamowienie {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize(),

                When(Potwierdzenie)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"{ctx.Saga.Login} potwierdzil zamowienie {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new AkceptacjaZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Unschedule(TO)
                .Finalize(),

                When(BrakPotwierdzenia)
                .ThenAsync(ctx => { return Console.Out.WriteLineAsync($"{ctx.Saga.Login} nie potwierdzil zamowienia {ctx.Message.CorrelationId}"); })
                .Respond(ctx => { return new OdrzucenieZamowienia() { CorrelationId = ctx.Saga.CorrelationId, Login = ctx.Saga.Login, Ilosc = ctx.Saga.Ilosc }; })
                .Finalize()
                );

            SetCompletedWhenFinalized();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var repo = new InMemorySagaRepository<RejestracjaZamowienie>();
            var saga = new RejestracjaSklep();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("amqps://wjfofomc:nl3uk8GF1UUfIClMIoCMRYN_KEPGQauF@rattlesnake.rmq.cloudamqp.com/wjfofomc"));
                sbc.ReceiveEndpoint("saga",
                    ep => ep.StateMachineSaga(saga, repo));
                sbc.UseInMemoryScheduler();
            });
            bus.Start();
            Console.WriteLine("Sklep otwarty");
            while (!Console.KeyAvailable) { }
            bus.Stop();
            Console.WriteLine("Sklep zamkniety");
        }
    }
}
