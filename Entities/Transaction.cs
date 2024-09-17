using System;

namespace JwtWalletAPI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string FromWalletAddress { get; set; }
        public string ToWalletAddress { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Currency { get; set; } = "NGN"; // Default currency
    }
}
