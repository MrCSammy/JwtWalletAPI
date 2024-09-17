/*using System;
using System.Threading.Tasks;
using JwtWalletAPI.Models;
using System.Collections.Generic;

namespace JwtWalletAPI.Services.Interfaces
{
    public interface IWalletService
    {
        Task<decimal> GetWalletBalanceAsync(string walletId);
        Task<Wallet> CreateWalletAsync(string userId);
        Task FundWalletAsync(Guid walletId, decimal amount);
        Task DebitWalletAsync(Guid walletId, decimal amount);
        Task TransferFundsAsync(Guid fromWalletId, Guid toWalletId, decimal amount);
        Task<List<Transaction>> GetTransactionHistoryAsync(Guid walletId, int pageNumber, int pageSize);
    }
}
*/