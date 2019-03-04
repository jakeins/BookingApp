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
        private readonly Mock<ApplicationUser> mockUser;
        private readonly Mock<AuthLoginDto> mockAuthLoginDto;
        private readonly Mock<AuthRegisterDto> mockAuthRegisterDto; 

        public AuthControllerTest()
        {
            mockNotificationService = new Mock<INotificationService>();
            mockUserService = new Mock<IUserService>();
            mockJwtService = new Mock<IJwtService>();
            mockUser = new Mock<ApplicationUser>();
            mockAuthLoginDto = new Mock<AuthLoginDto>();
            mockAuthRegisterDto = new Mock<AuthRegisterDto>();
        }

        [Fact]
        public async Task LoginWithCorrectParametersReturnsJwtTokensAsync()
        {
            var userClaims = It.IsAny<Claim[]>();
            var expectedAccessToken = "token";
            var expectedTokens = new AuthTokensDto { AccessToken = expectedAccessToken };
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
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTokens = Assert.IsType<AuthTokensDto>(okResult.Value);
            Assert.Equal(expectedTokens, actualTokens);
        }

        [Fact]
        public async Task LoginReturnsBadRequestWhenModelStateIsNotValidAsync()
        {
            var authLoginDto = new AuthLoginDto { Email = "", Password = "" };

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(authLoginDto);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task LoginReturnsBadRequestWhenUserCannotBeFound()
        {
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Email).Returns(It.IsAny<string>());
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Password).Returns(It.IsAny<string>());
            mockUserService.Setup(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email)).ReturnsAsync((ApplicationUser)null);

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(mockAuthLoginDto.Object);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task LoginReturnsBadRequestWhenPasswordNotMatchAsync()
        {
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Email).Returns(It.IsAny<string>());
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Password).Returns(It.IsAny<string>());
            mockUserService.Setup(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email)).ReturnsAsync(mockUser.Object);
            mockUserService.Setup(userService => userService.CheckPassword(mockUser.Object, mockAuthLoginDto.Object.Password)).ReturnsAsync(false);

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(mockAuthLoginDto.Object);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task LoginReturnsBadRequestWhenUserIsNotApprovedAsync()
        {
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Email).Returns(It.IsAny<string>());
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Password).Returns(It.IsAny<string>());
            mockUser.Setup(user => user.ApprovalStatus).Returns(false);
            mockUserService.Setup(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email)).ReturnsAsync(mockUser.Object);
            mockUserService.Setup(userService => userService.CheckPassword(mockUser.Object, mockAuthLoginDto.Object.Password)).ReturnsAsync(true);

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(mockAuthLoginDto.Object);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task LoginReturnsBadRequestWhenUserIsBlockedAsync()
        {
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Email).Returns(It.IsAny<string>());
            mockAuthLoginDto.Setup(authLoginDto => authLoginDto.Password).Returns(It.IsAny<string>());
            mockUser.Setup(user => user.ApprovalStatus).Returns(true);
            mockUser.Setup(user => user.IsBlocked).Returns(true);
            mockUserService.Setup(userService => userService.GetUserByEmail(mockAuthLoginDto.Object.Email)).ReturnsAsync(mockUser.Object);
            mockUserService.Setup(userService => userService.CheckPassword(mockUser.Object, mockAuthLoginDto.Object.Password)).ReturnsAsync(true);

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Login(mockAuthLoginDto.Object);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RegisterWithCorrectParametersCreateNewUserAndAddUserRoleAsync()
        {
            mockAuthRegisterDto.Setup(authRegisterDto => authRegisterDto.Email).Returns(It.IsAny<string>());
            mockAuthRegisterDto.Setup(authRegisterDto => authRegisterDto.UserName).Returns(It.IsAny<string>());
            mockAuthRegisterDto.Setup(authRegisterDto => authRegisterDto.Password).Returns(It.IsAny<string>());
            mockAuthRegisterDto.Setup(authRegisterDto => authRegisterDto.ConfirmPassword).Returns(It.IsAny<string>());
            mockUserService.Setup(userService => userService.CreateUser(mockUser.Object, It.IsAny<string>()));
            //mockUserService.Setup(userService => userService.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>()));

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Register(mockAuthRegisterDto.Object);

            var okResult = Assert.IsType<OkResult>(result);
            mockUserService.Verify(userService => userService.CreateUser(mockUser.Object, mockAuthRegisterDto.Object.Password));
            mockUserService.Verify(userService => userService.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task RegisterReturnsBadRequestWhenModelIsNotValidAsync()
        {
            var authRegisterDto = new AuthRegisterDto { Email = "", UserName = "", Password = "", ConfirmPassword = "1" };

            AuthController authController = new AuthController(mockNotificationService.Object, mockUserService.Object, mockJwtService.Object);
            var result = await authController.Register(authRegisterDto);

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            mockUserService.Verify(userService => userService.CreateUser(mockUser.Object, authRegisterDto.Password), Times.Never);
            mockUserService.Verify(userService => userService.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
