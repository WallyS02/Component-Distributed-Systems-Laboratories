using MassTransit.Serialization;
using System.Text;

namespace Wiadomosci
{
    public interface IPubl
    {
        int numer { get; set; }
    }

    public interface IOdpA
    {
        string kto { get; set; }
    }

    public interface IOdpB
    {
        string kto { get; set; }
    }

    public interface IUstaw
    {
        bool dziala { get; set; }
    }

    public class Klucz : SymmetricKey
    {
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
    }

    public class Dostawca : ISymmetricKeyProvider
    {
        private string k;
        public Dostawca(string _k)
        {
            k = _k;
        }
        public bool TryGetKey(string keyId, out SymmetricKey key)
        {
            var sk = new Klucz();
            sk.IV = Encoding.ASCII.GetBytes(keyId.Substring(0, 16));
            sk.Key = Encoding.ASCII.GetBytes(k);
            key = sk;
            return true;
        }
    }
}
