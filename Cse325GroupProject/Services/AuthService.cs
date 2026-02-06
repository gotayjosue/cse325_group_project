using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cse325GroupProject.DTOs;
using Cse325GroupProject.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Cse325GroupProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;

        public AuthService(IMongoClient mongoClient, IConfiguration configuration)
        {
            _configuration = configuration;
            // Assumes the database name is in .env or appsettings. If not, we'll need to specify it.
            // For now, let's assume "Cse325db" or get it from env if possible.
            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME") ?? "inventory-tracker";
            var database = mongoClient.GetDatabase(databaseName);
            _users = database.GetCollection<User>("Users");
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id!,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? "fallback_secret_key_long_enough_for_sha256");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id!),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("name", user.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
