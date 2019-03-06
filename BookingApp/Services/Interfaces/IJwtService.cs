using BookingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingApp.Services.Interfaces
{
    public interface IJwtService
    {
        DateTime ExpirationTime { get; }
        string GenerateJwtAccessToken(IEnumerable<Claim> claims);
        Task<Claim[]> GetClaimsAsync(ApplicationUser userInfo);
        string GenerateJwtRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken);
        Task LoginByRefreshTokenAsync(string userId, string refreshToken);
        Task<string> UpdateRefreshTokenAsync(string refreshToken, ClaimsPrincipal userPrincipal);
        Task DeleteRefreshTokenAsync(ClaimsPrincipal userPrincipal);
    }
}
