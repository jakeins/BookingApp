using System;
using Xunit;
using Moq;
using BookingApp.Services;
using System.Collections.Generic;
using BookingApp.Data.Models;
using BookingApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using System.Threading.Tasks;
using BookingApp.Services.Interfaces;
using BookingApp.DTOs.Resource;
using BookingApp.DTOs.User;

namespace BookingAppTests.Controllers
{
    public class UserControllerTest
    {
        #region GetUser(s)
        [Fact]
        public async void Get_WhenCalled_GetUserList()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            mockUserService.Setup(service => service.GetUsersList()).ReturnsAsync(GetUserList());
            var mockResourcesService = new Mock<IResourcesService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);

            // Act
            var result = await controller.GetAllUsers() as OkObjectResult;

            //Assert 
            var items = Assert.IsType<List<UserMinimalDto>>(result.Value);
            Assert.Equal(3, items.Count);
        }

        private IEnumerable<ApplicationUser> GetUserList()
        {
            List<ApplicationUser> user = new List<ApplicationUser>();
            user.Add(new ApplicationUser());
            user.Add(new ApplicationUser());
            user.Add(new ApplicationUser());
            return user;
        }

        [Fact]
        public async void Get_WhenCalled_GetUserById()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act
            var result = await mockController.Object.GetUserById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            Assert.NotNull(result.Value);
            Assert.IsType<UserMinimalDto>(result.Value);
            Assert.IsNotType<ApplicationUser>(result.Value);
        }
        
        [Fact]
        public async void Get_WhenCalled_GetUserByEmail()
        {
            // Arrange
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(mockApplicationUser.Object);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act
            var result = await mockController.Object.GetUserByEmail(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            Assert.NotNull(result.Value);
            Assert.IsType<UserMinimalDto>(result.Value);
            Assert.IsNotType<ApplicationUser>(result.Value);
        }

        [Fact]
        public async void Get_Exception_GetUserByEmail()
        {
            // Arrange
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserByEmail(It.IsAny<string>())).Throws(new  NullReferenceException("Can not find user with this email"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => controller.GetUserByEmail(It.IsAny<string>()));
            Assert.Equal("Can not find user with this email", ex.Message);
        }

        [Fact]
        public async void Get_WhenCalled_GetUserById_NotFound()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).Throws(new NullReferenceException("Can not find user with this id"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => mockController.Object.GetUserById(It.IsAny<string>()));
            Assert.Equal("Can not find user with this id", ex.Message);
        }
        #endregion

        #region Set and get User Role

        [Fact]
        public async void Get_OkObjectResult_WhenCalled_GetUserRoleById()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserRolesById(It.IsAny<string>())).ReturnsAsync(It.IsAny<IList<string>>());
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act
            var result = await mockController.Object.GetUserRoleById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_WhenCalled_GetUserRoleById()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserRolesById(It.IsAny<string>())).ReturnsAsync(GetRoles());
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act
            var result = await mockController.Object.GetUserRoleById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            var items = Assert.IsType<List<string>>(result.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal(GetRoles(), items);
        }

        private IList<string> GetRoles()
        {
            List<string> roles = new List<string>();
            roles.Add("User");
            roles.Add("Admin");
            return roles;
        }

        [Fact]
        public async void Get_UserException_WhenCalled_GetUserRoleById()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserRolesById(It.IsAny<string>())).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => mockController.Object.GetUserRoleById(It.IsAny<string>()));
            Assert.Equal("Default User Exception", ex.Message);
        }

        [Fact]
        public async void Get_WhenCalled_AddUserRoleByUserId()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);

            var mockUserRoleDto = new Mock<UserRoleDto>();
            mockUserRoleDto.SetupGet(x => x.Role).Returns("Admin");
            // Act
            var result = await controller.AddRole(It.IsAny<string>(), mockUserRoleDto.Object) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_UserException_WhenCalled_AddUserRoleByUserId()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);

            var mockUserRoleDto = new Mock<UserRoleDto>();
            mockUserRoleDto.SetupGet(x => x.Role).Returns("Admin");

            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => controller.AddRole(It.IsAny<string>(), mockUserRoleDto.Object));
            Assert.Equal("Default User Exception", ex.Message);
        }
        #endregion

        #region CreateUser
        [Fact]
        public async void Get_WhenCalled_CreateUser()
        {
            // Arrange
            var mockApplicaitonUser = new Mock<ApplicationUser>();
            var mockAuthRegisterDto = new Mock<AuthRegisterDto>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.CreateUser(mockApplicaitonUser.Object)).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act
            var result = await controller.CreateUser(mockAuthRegisterDto.Object) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region CreateAdmin
        [Fact]
        public async void Get_WhenCalled_CreateAdmin()
        {
            //await userService.CreateAdmin(appUser, user.Password);
            //ApplicationUser adminUser = await userService.GetUserByName(user.UserName);
            //await userService.AddUserRoleAsync(adminUser.Id, RoleTypes.Admin);
            // Arrange
            var mockApplicaitonUser = new Mock<ApplicationUser>();
            var mockAuthRegisterDto = new Mock<AdminRegisterDTO>();
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.CreateAdmin(mockApplicaitonUser.Object)).Returns(Task.CompletedTask);
            mockUserService.Setup(service => service.GetUserByName(It.IsAny<string>())).ReturnsAsync(mockApplicaitonUser.Object);
            mockUserService.Setup(service => service.AddUserRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act
            var result = await controller.CreateAdmin(mockAuthRegisterDto.Object) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region DeleteUser
        [Fact]
        public async void Get_WhenCalled_DeleteUserById()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.DeleteUser(It.IsAny<string>())).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act
            var result = await controller.DeleteUserById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_NullReferenceException_WhenCalled_DeleteUserById()
        {
            // Arrange
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.DeleteUser(It.IsAny<string>())).Throws(new NullReferenceException("Can not find user with this id"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => controller.DeleteUserById(It.IsAny<string>()));
            Assert.Equal("Can not find user with this id", ex.Message);
        }

        [Fact]
        public async void Get_UserException_WhenCalled_DeleteUserById()
        {
            // Arrange
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.DeleteUser(It.IsAny<string>())).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => controller.DeleteUserById(It.IsAny<string>()));
            Assert.Equal("Default User Exception", ex.Message);
        }
        #endregion

        #region GetResources
        [Fact]
        public async void Get_WhenCalled_GetResources()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockResourcesService = new Mock<IResourcesService>();
            mockResourcesService.Setup(services => services.ListByAssociatedUser(It.IsAny<string>())).ReturnsAsync(GetListResources());
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            // Act
            var result = await mockController.Object.GetResources(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            var items = Assert.IsType<List<ResourceMaxDto>>(result.Value);
            Assert.Equal(2, items.Count);
        }

        private IEnumerable<Resource> GetListResources()
        {
            List<Resource> resources = new List<Resource>();
            resources.Add(new Resource());
            resources.Add(new Resource());
            return resources;
        }
        #endregion

        #region UpdateUser

        [Fact]
        public async void Get_WhenCalled_UpdateUser()
        {
            // Arrange
            var mockUserUpdateDTO = new Mock<UserUpdateDTO>();
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).ReturnsAsync(mockApplicationUser.Object);
            mockUserService.Setup(service => service.UpdateUser(mockApplicationUser.Object)).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);
            // Act 
            var result = await mockController.Object.UpdateUser(mockUserUpdateDTO.Object, It.IsAny<string>()) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_NullReferenceException_WhenCalled_UpdateUser()
        {
            // Arrange
            var mockUserUpdateDTO = new Mock<UserUpdateDTO>();
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).Throws(new NullReferenceException("Can not find user with this id"));
            mockUserService.Setup(service => service.UpdateUser(mockApplicationUser.Object)).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);
            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => mockController.Object.UpdateUser(mockUserUpdateDTO.Object, It.IsAny<string>()));
            Assert.Equal("Can not find user with this id", ex.Message);  
        }

        [Fact]
        public async void Get_UserExceptions_WhenCalled_UpdateUser()
        {
            // Arrange
            var mockUserUpdateDTO = new Mock<UserUpdateDTO>();
            var mockApplicationUser = new Mock<ApplicationUser>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).ReturnsAsync(mockApplicationUser.Object);
            mockUserService.Setup(service => service.UpdateUser(mockApplicationUser.Object)).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();

            var mockNotificationService = new Mock<INotificationService>();
            var mockController = new Mock<UserController>(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);
            mockController.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>);
            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => mockController.Object.UpdateUser(mockUserUpdateDTO.Object, It.IsAny<string>()));
            Assert.Equal("Default User Exception", ex.Message);
        }

        #endregion

        #region UserApproval
        [Fact]
        public async void Get_WhenCalled_UserApproval()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserApproval(It.IsAny<string>(),  It.IsAny<bool>())).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act 
            var result = await controller.UserApproval(It.IsAny<string>() , It.IsAny<bool>() ) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_NullReferenceException_WhenCalled_UserApproval()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserApproval(It.IsAny<string>(), It.IsAny<bool>())).Throws(new NullReferenceException("Can not find user with this id"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => controller.UserApproval(It.IsAny<string>(), It.IsAny<bool>()));
            Assert.Equal("Can not find user with this id", ex.Message);
        }

        [Fact]
        public async void Get_UserExceptions_WhenCalled_UserApproval()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserApproval(It.IsAny<string>(), It.IsAny<bool>())).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => controller.UserApproval(It.IsAny<string>(), It.IsAny<bool>()));
            Assert.Equal("Default User Exception", ex.Message);
        }
        #endregion

        #region UserBlocking
        [Fact]
        public async void Get_WhenCalled_UserBlocking()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserBlocking(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act 
            var result = await controller.UserBlocking(It.IsAny<string>(), It.IsAny<bool>()) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_NullReferenceException_WhenCalled_UserBlocking()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserBlocking(It.IsAny<string>(), It.IsAny<bool>())).Throws(new NullReferenceException("Can not find user with this id"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => controller.UserBlocking(It.IsAny<string>(), It.IsAny<bool>()));
            Assert.Equal("Can not find user with this id", ex.Message);
        }

        [Fact]
        public async void Get_UserExceptions_WhenCalled_UserBlocking()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.UserBlocking(It.IsAny<string>(), It.IsAny<bool>())).Throws(new UserException("Default User Exception"));
            var mockResourcesService = new Mock<IResourcesService>();
            var mockBookingService = new Mock<IBookingsService>();
            var mockNotificationService = new Mock<INotificationService>();
            var controller = new UserController(mockUserService.Object, mockResourcesService.Object, mockBookingService.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<UserException>(() => controller.UserBlocking(It.IsAny<string>(), It.IsAny<bool>()));
            Assert.Equal("Default User Exception", ex.Message);
        }
        #endregion
    }
}
