﻿using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace _2
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Zad2
            var klinetOdkrywca = new DiscoveryClient(
                new UdpDiscoveryEndpoint("soap.udp://localhost:30703"));
            var lst = klinetOdkrywca.Find(new FindCriteria(typeof(IZadanie1))).Endpoints;
            klinetOdkrywca.Close();

            if (lst.Count > 0)
            {
                var adres = lst[0].Address;
                var klient = ChannelFactory<IZadanie1>
                    .CreateChannel(new NetNamedPipeBinding(), adres);
                Console.WriteLine(klient.ScalNapisy("Klient zad2", " dziala"));
                Console.ReadKey();
                ((IDisposable)klient).Dispose();
            }
        }
    }
}
