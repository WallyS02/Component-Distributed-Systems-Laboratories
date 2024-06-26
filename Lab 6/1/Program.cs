﻿using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace _1
{

    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }
    public class Zadanie1 : IZadanie1
    {
        public string ScalNapisy(string a, string b)
        {
            return a + b;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Zad1
            var host = new ServiceHost(typeof(Zadanie1));

            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.AddServiceEndpoint(
                new UdpDiscoveryEndpoint("soap.udp://localhost:30703"));

            host.AddServiceEndpoint(
                typeof(IZadanie1),
                new NetNamedPipeBinding(),
                "net.pipe://localhost/zadanie1");

            host.Open();
            Console.ReadKey();
            host.Close();
        }
    }
}
