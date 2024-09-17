using System.Threading.Tasks;

namespace JwtWalletAPI.Services.Interfaces
{
    public interface ICurrencyService
    {
        decimal GetConversionRate(string fromCurrency, string toCurrency);
    }

}
