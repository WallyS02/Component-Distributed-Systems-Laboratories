using klasac_test;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("klasac_test.dll")]
    public static extern uint Test(string napis);

    static void Main(string[] args)
    {
        IKlasa klasa = new Klasa();
        klasa.Test("klasac dziala w c#");
    }
}