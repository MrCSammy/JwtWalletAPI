using JwtWalletAPI.Services.Interfaces;

namespace JwtWalletAPI.Services
{

    public class CurrencyService : ICurrencyService
    {
        public decimal GetConversionRate(string fromCurrency, string toCurrency)
        {
            // Dummy rates for simplicity
            if (fromCurrency == "USD" && toCurrency == "NGN") return 760;
            if (fromCurrency == "NGN" && toCurrency == "USD") return 0.0013m;

            return 1; // Default for same currency
        }
    }
}
