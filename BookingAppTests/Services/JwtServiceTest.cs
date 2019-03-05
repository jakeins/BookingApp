using BookingApp.Data.Models;
using BookingApp.Repositories;
using BookingApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace BookingAppTests.Services
{
    public class JwtServiceTest
    {
        private readonly Mock<IUserRefreshTokenRepository> mockRefreshRepository;
        private readonly Mock<IUserService> mockUserService;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly Mock<ClaimsPrincipal> mockClaimsPrincipal;
        private readonly Mock<UserRefreshToken> mockUserRefreshToken;

        public JwtServiceTest()
        {
            mockRefreshRepository = new Mock<IUserRefreshTokenRepository>();
            mockUserService = new Mock<IUserService>();
            mockConfiguration = new Mock<IConfiguration>();
            mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockUserRefreshToken = new Mock<UserRefreshToken>();
        }

        [Fact]
        public void GenerateJwtRefreshTokenReturnsStringLength44()
        {
            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var refreshToken = jwtService.GenerateJwtRefreshToken();

            Assert.Equal(44, refreshToken.Length);
        }

        [Fact]
        public async void DeleteRefreshTokenThrowsExceptionWhenClaimsPrincipalIsInvalidAsync()
        {
            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtService.DeleteRefreshTokenAsync(mockClaimsPrincipal.Object));
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.DeleteAsync(mockUserRefreshToken.Object.Id), Times.Never);
        }

        [Fact]
        public async void DeleteRefreshTokenMustDeleteRefreshTokenByUserEmailAsync()
        {
            var mockClaim = new Mock<Claim>("uid", "id");
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(mockUserRefreshToken.Object);

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            await jwtService.DeleteRefreshTokenAsync(mockClaimsPrincipal.Object);

            mockRefreshRepository.Verify(refreshRepository => refreshRepository.DeleteAsync(mockUserRefreshToken.Object.Id), Times.Once);
        }

        [Fact]
        public async void LoginByRefreshTokenMustUpdateRefreshTokenWhenItExisitsAsync()
        {
            mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(mockUserRefreshToken.Object);

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            await jwtService.LoginByRefreshTokenAsync("id", "token");

            mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(mockUserRefreshToken.Object), Times.Once);
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.CreateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }

        [Fact]
        public async void LoginByRefreshTokenMustCreateRefreshTokenWhenUserNotHaveYetAsync()
        {
            mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserRefreshToken)null);

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            await jwtService.LoginByRefreshTokenAsync("id", "token");

            mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.CreateAsync(It.IsAny<UserRefreshToken>()), Times.Once);
        }

        [Fact]
        public async void UpdateRefreshTokenThrowsExceptionWhenClaimsPrincipalIsInvalidAsync()
        {
            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtService.UpdateRefreshTokenAsync("token", mockClaimsPrincipal.Object));
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }

        [Fact]
        public async void UpdateRefreshTokenThrowsExceptionWhenRefreshTokensIsNotEqualAsync()
        {
            var mockClaim = new Mock<Claim>("uid", "id");
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(mockUserRefreshToken.Object);

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtService.UpdateRefreshTokenAsync("token", mockClaimsPrincipal.Object));
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }

        [Fact]
        public async void UpdateRefreshTokenMustUpdateRefreshTokenAndReturnNewRefreshTokenAsync()
        {
            var userRefreshToken = new UserRefreshToken { RefreshToken = "token" };
            var mockClaim = new Mock<Claim>("uid", "id");
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(userRefreshToken);

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualRefreshToken = await jwtService.UpdateRefreshTokenAsync("token", mockClaimsPrincipal.Object);
            
            mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(userRefreshToken), Times.Once);
        }
    }
}
