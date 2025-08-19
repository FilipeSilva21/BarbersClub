using System.Security.Claims;
using BarbersClub.Business.DTOs;
using Repository.Models;

namespace BarbersClub.Business.Services.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterRequest request);

    Task<(string? token, User? user)> LoginAsync(UserLoginRequest request);

    public string GenerateToken(IEnumerable<Claim> claims);
}