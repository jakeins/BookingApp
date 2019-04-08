using BookingApp;
using BookingApp.Data.Models;
using BookingApp.DTOs.Resource;
using BookingAppIntegrationTests.TestingUtilities;
using BookingAppIntegrationTests.Tests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static BookingAppIntegrationTests.TestingUtilities.AuthUtils;

namespace BookingAppIntegrationTests.Scenarios
{
    public class ResourcesControllerIntergationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        readonly HttpClient httpClient;
        readonly string apiPath ="api/resources";
        readonly string appJson = "application/json";

        public ResourcesControllerIntergationTests(CustomWebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        #region READ tests
        [Theory]
        [InlineData("")]
        [InlineData("/1")]
        public async Task GetEndpoints_ReturnSuccsessfullJson(string subPath)
        {
            //Arrange
            var path = apiPath + subPath;

            //Act
            var response = await httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(this.appJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task List_ReturnsResources()
        {
            //Arrange
            var path = apiPath;

            //Act
            var response = await httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ResourceBriefDto>>(stringResponse);

            // Assert
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Single_ReturnsResource(int id)
        {
            //Arrange
            var path = apiPath + "/" + id;

            //Act
            var response = await httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResourceMaxDto>(stringResponse);

            // Assert
            Assert.False(string.IsNullOrEmpty(result.Title));
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(13)]
        public async Task Single_ReturnsError_OnInactive_ForRegularUser(int id)
        {
            //Arrange
            var path = apiPath + "/" + id;

            //Act
            var response = await httpClient.GetAsync(path);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Single_ReturnsNotFound_OnNotExistingId()
        {
            //Arrange
            var path = apiPath + "/999999";

            //Act
            var response = await httpClient.GetAsync(path);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region Create() tests
        [Fact]
        public async Task Create_ReturnsCreated()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_ForRegularUser()
        {
            //Arrange
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_OnFaultyData()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(NewFaultyResourceModel);
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region Update() tests
        [Fact]
        public async Task Update_ReturnsOK()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsUnauthorized_ForRegularUser()
        {
            //Arrange
            var path = apiPath + "/1";

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PutAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_OnFaultyData()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath + "/1";

            //Act
            var content = JsonConvert.SerializeObject(NewFaultyResourceModel);
            var response = await httpClient.PutAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_OnNotExistingId()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath + "/999999";

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PutAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region Delete() tests
        [Fact]
        public async Task Delete_ReturnsOK()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsUnauthorized_ForRegularUser()
        {
            //Arrange
            var path = apiPath + "/1";

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.DeleteAsync(path);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_OnNotExistingId()
        {
            //Arrange
            httpClient.AddBearerFor(UserType.ActiveAdmin);
            var path = apiPath + "/999999";

            //Act
            var content = JsonConvert.SerializeObject(NewNormalResourceModel);
            var response = await httpClient.DeleteAsync(path);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion



        #region Resource Integeration Test Utilities
        Resource NewNormalResourceModel => new Resource { Title = "Completely normal resource", RuleId = 1, IsActive = true };
        Resource NewFaultyResourceModel => new Resource { Title = "", RuleId = -200 };
        #endregion
    }
}