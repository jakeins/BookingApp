﻿using BookingApp;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingAppIntegrationTests.TestingUtilities;
using BookingAppIntegrationTests.Tests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace BookingAppIntegrationTests.Scenarios
{
    public class UserControllerIntegradtionTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public UserControllerIntegradtionTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        #region CreateUserTest
        [Theory]
        [InlineData("api/user")]
        public async Task CreateFolderTest(string url)
        {
            var content = JsonConvert.SerializeObject(new AuthRegisterDto { Email = "example@email.com", UserName = "UserName", Password = "Password", ConfirmPassword = "Password" });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/user")]
        public async Task Create_ReturnsBadRequest_OnFaultyData(string url)
        {
            //Arrange


            //Act
            var content = JsonConvert.SerializeObject(new AuthRegisterDto { Email = "example@email.com", UserName = "UserName", Password = "Password", ConfirmPassword = "Fail" });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region UpdateUserTest
        [Theory]
        [InlineData("api/user")]
        public async Task Update_OwnProfile_RegularUser(string url)
        {
            await AuthUtils.AddUsersBearer(_client);
            url += "/" + AuthUtils.UserId;// user id Lion(regular user) 
            
            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO { Email = "lion@user.cow", UserName = "Lions"});
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/user")]
        public async Task Update_OwnProfile_RegularUser_NonAuthorize(string url)
        {
            await AuthUtils.AddUsersBearer(_client);
            url += "/someOtherId";
            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO { Email = "lion@user.cow", UserName = "Lions" });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("api/user")]
        public async Task Update_OtherProfile_RegularUser_NonAuthorize(string url)
        {
            url += "/undefined";
            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO { Email = "lion@user.cow", UserName = "Lions" });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("api/user")]
        public async Task Update_UserProfile_Admin(string url)
        {
            await AuthUtils.AddAdminsBearer(_client);
            url += "/" + AuthUtils.UserId;// user id SuperAdmin(Admin) 

            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO { Email = "lion@user.cow", UserName = "Lions" });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        #endregion

        #region DeleteUserTest
        [Theory]
        [InlineData("api/user")]
        public async Task Delete_User_ByAdmin(string url)
        {
            await AuthUtils.AddAdminsBearer(_client);
            url += "/" + AuthUtils.UserId;// user id Lion(regular user) 

            //Arrange
            var response = await _client.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        #endregion
 
    }
}