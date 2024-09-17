using Microsoft.AspNetCore.Mvc;
using JwtWalletAPI.Services;
using JwtWalletAPI.Services.Interfaces;

namespace JwtWalletAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("convert")]
        public IActionResult ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            var rate = _currencyService.GetConversionRate(fromCurrency, toCurrency);
            var convertedAmount = amount * rate;
            return Ok(new { fromCurrency, toCurrency, amount, convertedAmount });
        }
    }
}
