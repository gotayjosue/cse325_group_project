using Cse325GroupProject.DTOs;

namespace Cse325GroupProject.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }
}
