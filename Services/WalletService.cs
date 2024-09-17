/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtWalletAPI.Data;
using JwtWalletAPI.Models;
using JwtWalletAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JwtWalletAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(ApplicationDbContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Wallet> CreateWalletAsync(string userId)
        {
            var wallet = new Wallet { UserId = userId };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Wallet {wallet.WalletAddress} created for user {userId}");
            return wallet;
        }
        public async Task<decimal> GetWalletBalanceAsync(string walletId)
        {
            if (string.IsNullOrEmpty(walletId))
            {
                throw new ArgumentNullException(nameof(walletId), "walletId cannot be null or empty.");
            }

            // Convert the string walletId to Guid
            Guid walletGuid;
            if (!Guid.TryParse(walletId, out walletGuid))
            {
                throw new ArgumentException("Invalid walletId format.", nameof(walletId));
            }

            // Retrieve the wallet from the database
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletId == walletGuid);

            if (wallet == null)
            {
                throw new KeyNotFoundException("Wallet not found.");
            }

            // Return the balance
            return wallet.Balance;
        }

    public async Task FundWalletAsync(Guid walletId, decimal amount)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            wallet.Balance += amount;

            var transaction = new Transaction
            {
                WalletId = walletId,
                Amount = amount,
                TransactionType = TransactionType.Credit,
                Currency = wallet.Currency,
                Description = "Wallet funded"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Wallet {wallet.WalletAddress} funded with {amount} {wallet.Currency}");
        }

        public async Task DebitWalletAsync(Guid walletId, decimal amount)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            if (wallet.Balance < amount)
            {
                throw new Exception("Insufficient balance");
            }

            wallet.Balance -= amount;

            var transaction = new Transaction
            {
                WalletId = walletId,
                Amount = amount,
                TransactionType = TransactionType.Debit,
                Currency = wallet.Currency,
                Description = "Wallet debited"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Wallet {wallet.WalletAddress} debited with {amount} {wallet.Currency}");
        }

        public async Task TransferFundsAsync(Guid fromWalletId, Guid toWalletId, decimal amount)
        {
            var fromWallet = await _context.Wallets.FindAsync(fromWalletId);
            var toWallet = await _context.Wallets.FindAsync(toWalletId);

            if (fromWallet == null || toWallet == null)
            {
                throw new Exception("One or both wallets not found");
            }

            if (fromWallet.Balance < amount)
            {
                throw new Exception("Insufficient balance in the source wallet");
            }

            fromWallet.Balance -= amount;
            toWallet.Balance += amount;

            var fromTransaction = new Transaction
            {
                WalletId = fromWalletId,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                Currency = fromWallet.Currency,
                Description = $"Transferred {amount} to wallet {toWallet.WalletAddress}"
            };

            var toTransaction = new Transaction
            {
                WalletId = toWalletId,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                Currency = toWallet.Currency,
                Description = $"Received {amount} from wallet {fromWallet.WalletAddress}"
            };

            _context.Transactions.AddRange(fromTransaction, toTransaction);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Transferred {amount} from wallet {fromWallet.WalletAddress} to {toWallet.WalletAddress}");
        }

        public async Task<List<Transaction>> GetTransactionHistoryAsync(Guid walletId, int pageNumber, int pageSize)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
*/