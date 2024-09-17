using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtWalletAPI.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; } // Change to WalletId to avoid conflict with ApplicationUser Id
        [Required]
        public string WalletAddress { get; set; }
        public decimal Balance { get; set; }

        // Foreign Key for ApplicationUser
        [Required]
        public string UserId { get; set; } // Ensure this matches your Identity column for ApplicationUser

        // Navigation property for the user
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
