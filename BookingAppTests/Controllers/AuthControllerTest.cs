using BookingApp.Controllers;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BookingAppTests.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<INotificationService> mockNotificationService;
        private readonly Mock<IUserService> mockUserService;
        private readonly Mock<IJwtService> mockJwtService;

        public AuthControllerTest()
        {
            mockNotificationService = new Mock<INotificationService>();
            mockUserService = new Mock<IUserService>();
            mockJwtService = new Mock<IJwtService>();
        }

        [Fact]
        public async Task LoginWithCorrectParametersAsync()
        {
            var userClaims = It.IsAny<Claim[]>();
            var expectedAccessToken = "token"; // It.IsAny<string>() don't work and returns null
            var mockUser = new Mock<ApplicationUser>();
            var mockAuthLoginDto = new Mock<AuthLoginDto>();
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Email).Returns(It.IsAny<string>());
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Password).Returns(It.IsAny<string>());
            mockUser.Setup(user => user.ApprovalStatus).Returns(true);
            mockUser.Setup(user => user.IsBlocked).Returns(false);
            mockUserService.Setup(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email)).ReturnsAsync(mockUser.Object);
            mockUserService.Setup(userService => userService.CheckPassword(mockUser.Object, mockAuthLoginDto.Object.Password)).ReturnsAsync(true);
            mockJwtService.Setup(jwtService => jwtService.GetClaimsAsync(mockUser.Object)).ReturnsAsync(userClaims);
            mockJwtService.Setup(jwtService => jwtService.GenerateJwtAccessToken(userClaims)).Returns(expectedAccessToken);

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(mockAuthLoginDto.Object);

            mockUserService.Verify(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email));
            mockUserService.Verify(userService => userService.CheckPassword(mockUser.Object, mockAuthLoginDto.Object.Password));
            mockJwtService.Verify(jwtService => jwtService.GetClaimsAsync(mockUser.Object));
            mockJwtService.Verify(jwtService => jwtService.GenerateJwtAccessToken(userClaims));
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualAccessToken = Assert.IsAssignableFrom<string>(okResult.Value);
            Assert.Equal(expectedAccessToken, actualAccessToken);
        }
    }
}
