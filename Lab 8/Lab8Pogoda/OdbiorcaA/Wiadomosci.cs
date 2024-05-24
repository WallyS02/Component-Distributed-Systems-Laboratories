
namespace Wiadomosci
{
    public interface ITemperatura
    {
        double temperatura { get; set; }
    }

    public interface ICisnienie
    {
        double cisnienie { get; set; }
    }

    public interface ITempICisn : ITemperatura, ICisnienie
    {
    }
}
