using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Routing;

namespace Router
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var routePath1 = "net.pipe://localhost/zadanie6-1";
            var routePath2 = "net.pipe://localhost/zadanie6-2";
            var routeAdres = "net.pipe://localhost/router";

            var host = new ServiceHost(typeof(RoutingService));
            host.AddServiceEndpoint(
                typeof(IRequestReplyRouter),
                new NetNamedPipeBinding(),
                routeAdres);

            var routeConfig = new RoutingConfiguration();
            var contract = ContractDescription.GetContract(typeof(IRequestReplyRouter));
            var klient1 = new ServiceEndpoint(
                contract,
                new NetNamedPipeBinding(),
                new EndpointAddress(routePath1));
            var klient2 = new ServiceEndpoint(
                contract,
                new NetNamedPipeBinding(),
                new EndpointAddress(routePath2));

            var list = new List<ServiceEndpoint>
            {
                klient1,
                klient2
            };

            routeConfig.FilterTable.Add(new MatchAllMessageFilter(), list);
            host.Description.Behaviors.Add(new RoutingBehavior(routeConfig));

            host.Open();
            Console.WriteLine("Router");
            Console.ReadKey();
            host.Close();
        }
    }
}
