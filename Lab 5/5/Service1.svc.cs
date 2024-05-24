using System;
using KSR_WCF2;
using System.ServiceModel;

namespace _5
{
    public class Service1 : IZadanie5, IZadanie6
    {
        public string ScalNapisy(string a, string b)
        {
            return a + b;
        }
        public void Dodaj(int a, int b)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IZadanie6Zwrotny>();
            callback.Wynik(a + b);
        }
    }
}
