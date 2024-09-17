using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtWalletAPI.Models;
using JwtWalletAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtWalletAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; // Database context for wallet creation

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }
        // User Registration Endpoint with automatic wallet creation
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Automatically create wallet after successful registration
                    var wallet = new Wallet
                    {
                        UserId = user.Id,
                        WalletAddress = Guid.NewGuid().ToString(), // Unique wallet address
                        Balance = 0 // Default balance
                    };

                    _context.Wallets.Add(wallet);
                    await _context.SaveChangesAsync();

                    // Generate token with wallet and user data
                    var token = GenerateJwtToken(user, wallet);

                    return Ok(new { token });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Invalid registration details.");
        }

        // User Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Retrieve wallet associated with the user
                var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);

                // Generate JWT token with wallet and user data
                var token = GenerateJwtToken(user, wallet);

                return Ok(new { token});
            }

            return Unauthorized("Invalid login credentials.");
        }

        // Helper Method to Generate JWT Token
        private string GenerateJwtToken(ApplicationUser user, Wallet wallet)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id), // Store userId in the token
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("WalletAddress", wallet.WalletAddress),
        new Claim("Balance", wallet.Balance.ToString()),
        new Claim("UserName", user.UserName)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
