using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtWalletAPI.Data;
using JwtWalletAPI.Models;

namespace JwtWalletAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetBalanceFromToken()
        {
            var balanceClaim = User.Claims.FirstOrDefault(c => c.Type == "Balance")?.Value;
            return balanceClaim;
        }
        [HttpGet("WalletAddress")]
        public IActionResult GetWalletAddress()
        {
            var MyWalletAddress = User.Claims.FirstOrDefault(c => c.Type == "WalletAddress")?.Value;
            return Ok(MyWalletAddress);
        }

        [HttpPost("fund")]
        public async Task<IActionResult> FundWallet([FromBody] decimal amount)
        {
            var balanceClaim = GetBalanceFromToken();
            if (balanceClaim == null)
                return Unauthorized("Invalid token or balance not found");

            if (!decimal.TryParse(balanceClaim, out var currentBalance))
                return BadRequest("Invalid balance claim");

            var walletAddress = User.Claims.FirstOrDefault(c => c.Type == "WalletAddress")?.Value;

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletAddress == walletAddress);

            if (wallet == null)
                return NotFound("Wallet not found");

            wallet.Balance += amount;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Wallet funded successfully", wallet.Balance });
        }

        [HttpPost("debit")]
        public async Task<IActionResult> DebitWallet([FromBody] decimal amount)
        {
            var balanceClaim = GetBalanceFromToken();
            if (balanceClaim == null)
                return Unauthorized("Invalid token or balance not found");

            if (!decimal.TryParse(balanceClaim, out var currentBalance))
                return BadRequest("Invalid balance claim");

            var walletAddress = User.Claims.FirstOrDefault(c => c.Type == "WalletAddress")?.Value;

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletAddress == walletAddress);

            if (wallet == null)
                return NotFound("Wallet not found");

            if (wallet.Balance < amount)
                return BadRequest("Insufficient balance");

            wallet.Balance -= amount;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Wallet debited successfully", wallet.Balance });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferModel transfer)
        {
            var senderWalletAddress = User.Claims.FirstOrDefault(c => c.Type == "WalletAddress")?.Value;

            if (senderWalletAddress == null)
                return Unauthorized("Invalid token or wallet address not found");

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletAddress == senderWalletAddress);

            if (senderWallet == null)
                return NotFound("Sender's wallet not found");

            if (senderWallet.Balance < transfer.Amount)
                return BadRequest("Insufficient balance");

            var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletAddress == transfer.ReceiverWalletAddress);

            if (receiverWallet == null)
                return NotFound("Receiver's wallet not found");

            senderWallet.Balance -= transfer.Amount;
            receiverWallet.Balance += transfer.Amount;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Transfer successful" });
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            var balanceClaim = GetBalanceFromToken();

            if (balanceClaim == null)
                return Unauthorized("Invalid token or balance not found");

            if (!decimal.TryParse(balanceClaim, out var balance))
                return BadRequest("Invalid balance claim");

            return Ok(new { Balance = balance });
        }
    }
    public class TransferModel
    {
        public string ReceiverWalletAddress { get; set; } // Wallet address of the recipient
        public decimal Amount { get; set; } // Amount to transfer
    }    
}
