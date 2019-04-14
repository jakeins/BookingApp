using BookingApp;
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
using static BookingAppIntegrationTests.TestingUtilities.AuthUtils;

namespace BookingAppIntegrationTests.Scenarios
{
    public class UserControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        readonly HttpClient httpClient;
        const string apiUserPrePath = "api/user/";
        const string appJson = "application/json";

        public UserControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        #region GetUserTest
        [Fact]
        public async Task Get_User_OnId()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            string url = apiUserPrePath + httpClient.GetTestToken(UserType.ActiveUser).UserID;

            //Arrange
            var response = await httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserMinimalDto>(stringResponse);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(result.Id));
        }

        [Fact]
        public async Task Get_User_OnName()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            string url = apiUserPrePath + "user-name/" + httpClient.GetTestToken(UserType.ActiveUser).UserName;

            //Arrange
            var response = await httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserMinimalDto>(stringResponse);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(result.Id));
        }

        [Fact]
        public async Task Get_User_OnEmail()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            string url = apiUserPrePath + "email/" + httpClient.GetTestToken(UserType.ActiveUser).UserEmail;

            //Arrange
            var response = await httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserMinimalDto>(stringResponse);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(result.Id));
        }

        [Fact]
        public async Task Get_Users_ByAdmin()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);

            //Arrange
            var response = await httpClient.GetAsync("api/users");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<UserMinimalDto>>(stringResponse);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.Count > 0);
        }
        #endregion

        #region CreateUserTest
        [Fact]
        public async Task Create_User()
        {
            var content = JsonConvert.SerializeObject(new AuthRegisterDto {
                Email = "example@email.com",
                UserName = "UserName",
                Password = "Password",
                ConfirmPassword = "Password" });
            var response = await httpClient.PostAsync(apiUserPrePath, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_OnFaultyData()
        {
            //Arrange
            var content = JsonConvert.SerializeObject(new AuthRegisterDto {
                Email = "example@email.com",
                UserName = "UserName",
                Password = "Password",
                ConfirmPassword = "Fail" });

            //Act
            var response = await httpClient.PostAsync(apiUserPrePath, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region UpdateUserTest
        [Fact]
        public async Task Update_User_ByHimself()
        {
            httpClient.AddBearerFor(UserType.UpdatableUser);
            var updUserToken = httpClient.GetTestToken(UserType.UpdatableUser);
            string url = apiUserPrePath + updUserToken.UserID;

            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO
            {
                UserName = updUserToken.UserName + "A"
            });
            var response = await httpClient.PutAsync(url, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_User_ByAnotherUser()
        {
            httpClient.AddBearerFor(UserType.ActiveUser);
            var updatableUserToken = httpClient.GetTestToken(UserType.UpdatableUser);
            string url = apiUserPrePath + updatableUserToken.UserID;

            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO
            {
                UserName = updatableUserToken.UserName + "A"
            });
            var response = await httpClient.PutAsync(url, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }



        [Fact]
        public async Task Update_User_ByAdmin()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var updatableUserToken = httpClient.GetTestToken(UserType.UpdatableUser);
            string url = apiUserPrePath + updatableUserToken.UserID;

            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO
            {
                UserName = updatableUserToken.UserName + "A"
            });
            var response = await httpClient.PutAsync(url, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_User_ByAnonymous()
        {
            var updatableUserToken = httpClient.GetTestToken(UserType.UpdatableUser);
            string url = apiUserPrePath + updatableUserToken.UserID;

            //Arrange
            var content = JsonConvert.SerializeObject(new UserUpdateDTO
            {
                UserName = updatableUserToken.UserName + "A"
            });
            var response = await httpClient.PutAsync(url, new StringContent(content, Encoding.UTF8, appJson));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        #endregion

        #region Delete_User
        [Fact]
        public async Task Delete_User_ByAdmin()
        {
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var deletableUserToken = httpClient.GetTestToken(UserType.DeletableUser);
            string url = apiUserPrePath + deletableUserToken.UserID;

            //Arrange
            var response = await httpClient.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_NotAutorized()
        {
            string url = apiUserPrePath + "undefined";

            //Arrange
            var response = await httpClient.DeleteAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        #endregion

    }
}
