using BookingApp.Data.Models;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly Mock<ApplicationUser> mockUser;

        public JwtServiceTest()
        {
            mockRefreshRepository = new Mock<IUserRefreshTokenRepository>();
            mockUserService = new Mock<IUserService>();
            mockConfiguration = new Mock<IConfiguration>();
            mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockUserRefreshToken = new Mock<UserRefreshToken>();
            mockUser = new Mock<ApplicationUser>();
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

        [Fact]
        public async void GetClaimsMustWriteUserNameInClaims()
        {
            var roles = new List<string> { "somerole" };
            mockUserService.Setup(userService => userService.GetUserRoles(mockUser.Object)).ReturnsAsync(roles);
            mockUser.SetupGet(user => user.UserName).Returns("userName");
            mockUser.SetupGet(user => user.Email).Returns("email");
            mockUser.SetupGet(user => user.Id).Returns("id");

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualClaims = await jwtService.GetClaimsAsync(mockUser.Object);
            var existsUserName = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == "userName");

            mockUserService.Verify();
            mockUser.Verify();
            Assert.True(existsUserName);
        }

        [Fact]
        public async void GetClaimsMustWriteEmailInClaims()
        {
            var roles = new List<string> { "somerole" };
            mockUserService.Setup(userService => userService.GetUserRoles(mockUser.Object)).ReturnsAsync(roles);
            mockUser.SetupGet(user => user.UserName).Returns("userName");
            mockUser.SetupGet(user => user.Email).Returns("email");
            mockUser.SetupGet(user => user.Id).Returns("id");

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualClaims = await jwtService.GetClaimsAsync(mockUser.Object);
            var expectedClaim = new Claim(JwtRegisteredClaimNames.Email, "email");
            var existsEmail = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "email");

            mockUserService.Verify();
            mockUser.Verify();
            Assert.True(existsEmail);
        }

        [Fact]
        public async void GetClaimsMustWriteIdInClaims()
        {
            var roles = new List<string> { "somerole" };
            mockUserService.Setup(userService => userService.GetUserRoles(mockUser.Object)).ReturnsAsync(roles);
            mockUser.SetupGet(user => user.UserName).Returns("userName");
            mockUser.SetupGet(user => user.Email).Returns("email");
            mockUser.SetupGet(user => user.Id).Returns("id");

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualClaims = await jwtService.GetClaimsAsync(mockUser.Object);
            var expectedClaim = new Claim("uid", "id");
            var existsId = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == "uid" && claim.Value == "id");

            mockUserService.Verify();
            mockUser.Verify();
            Assert.True(existsId);
        }

        [Fact]
        public async void GetClaimsMustWriteRoleInClaims()
        {
            var roles = new List<string> { "somerole" };
            mockUserService.Setup(userService => userService.GetUserRoles(mockUser.Object)).ReturnsAsync(roles);
            mockUser.SetupGet(user => user.UserName).Returns("userName");
            mockUser.SetupGet(user => user.Email).Returns("email");
            mockUser.SetupGet(user => user.Id).Returns("id");

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualClaims = await jwtService.GetClaimsAsync(mockUser.Object);
            var existsRole = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == ClaimTypes.Role && claim.Value == "somerole");

            mockUserService.Verify();
            mockUser.Verify();
            Assert.True(existsRole);
        }

        [Fact]
        public async void GetClaimsMustGenerateJtiInClaims()
        {
            var roles = new List<string> { "somerole" };
            mockUserService.Setup(userService => userService.GetUserRoles(mockUser.Object)).ReturnsAsync(roles);
            mockUser.SetupGet(user => user.UserName).Returns("userName");
            mockUser.SetupGet(user => user.Email).Returns("email");
            mockUser.SetupGet(user => user.Id).Returns("id");

            JwtService jwtService = new JwtService(mockRefreshRepository.Object, mockUserService.Object, mockConfiguration.Object);
            var actualClaims = await jwtService.GetClaimsAsync(mockUser.Object);
            var existsJti = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Jti);

            mockUserService.Verify();
            mockUser.Verify();
            Assert.True(existsJti);
        }
    }
}
