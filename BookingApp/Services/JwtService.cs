using BookingApp.Data.Models;
using BookingApp.Repositories;
using BookingApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class JwtService : IJwtService
    {
        private readonly IUserRefreshTokenRepository refreshRepository;
        private readonly IUserService userService;
        private readonly IConfiguration configuration;

        public JwtService(IUserRefreshTokenRepository refreshRepository, IUserService userService, IConfiguration configuration)
        {
            this.refreshRepository = refreshRepository;
            this.userService = userService;
            this.configuration = configuration;
        }

        public DateTime ExpirationTime => DateTime.Now.AddMinutes(120);

        public async Task DeleteRefreshTokenAsync(ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.HasClaim(c => c.Type == "uid"))
            {
                throw new SecurityTokenException("Invalid access token");
            }

            var userId = userPrincipal.FindFirst(c => c.Type == "uid").Value;
            var refreshToken = await refreshRepository.GetByUserIdAsync(userId);
            if (refreshToken == null)
            {
                throw new SecurityTokenException("Invalid access token");
            }

            await refreshRepository.DeleteAsync(refreshToken.Id);
        }

        public string GenerateJwtAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: configuration["Jwt:Issuer"],
              audience: configuration["Jwt:Audience"],
              claims: claims,
              expires: ExpirationTime,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJwtRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<Claim[]> GetClaimsAsync(ApplicationUser userInfo)
        {
            var roles = await userService.GetUserRoles(userInfo);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", userInfo.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims.ToArray();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid access token");

            return principal;
        }

        public async Task LoginByRefreshTokenAsync(string userId, string refreshToken)
        {
            var userRefreshToken = await refreshRepository.GetByUserIdAsync(userId);
            if (userRefreshToken != null)
            {
                userRefreshToken.RefreshToken = refreshToken;
                userRefreshToken.ExpireOn = DateTime.Now.AddMonths(3);
                await refreshRepository.UpdateAsync(userRefreshToken);
            }
            else
            {
                userRefreshToken = new UserRefreshToken
                {
                    UserId = userId,
                    RefreshToken = refreshToken,
                    ExpireOn = DateTime.Now.AddMonths(3)
                };
                await refreshRepository.CreateAsync(userRefreshToken);
            }
        }

        public async Task<string> UpdateRefreshTokenAsync(string oldRefreshToken, ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.HasClaim(c => c.Type == "uid"))
            {
                throw new SecurityTokenException("Invalid access token");
            }

            var userId = userPrincipal.FindFirst(c => c.Type == "uid").Value;
            var savedRefreshToken = await refreshRepository.GetByUserIdAsync(userId);
            if (oldRefreshToken != savedRefreshToken?.RefreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newRefreshToken = GenerateJwtRefreshToken();

            savedRefreshToken.RefreshToken = newRefreshToken;
            savedRefreshToken.ExpireOn = DateTime.Now.AddMonths(3);
            await refreshRepository.UpdateAsync(savedRefreshToken);
            return newRefreshToken;
        }
    }
}