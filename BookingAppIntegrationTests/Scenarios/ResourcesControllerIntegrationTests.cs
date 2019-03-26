using BookingApp;
using BookingApp.Data.Models;
using BookingApp.DTOs.Resource;
using BookingAppIntegrationTests.TestingUtilities;
using BookingAppIntegrationTests.Tests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            await AuthUtils.AddAdminsBearer(httpClient);
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
            await AuthUtils.AddAdminsBearer(httpClient);
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
            await AuthUtils.AddAdminsBearer(httpClient);
            var path = apiPath + "/" + id;

            //Act
            var response = await httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResourceMaxDto>(stringResponse);

            // Assert
            Assert.IsAssignableFrom<ResourceMaxDto>(result);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(13)]
        public async Task Single_ReturnsError_OnInactive_ForRegularUser(int id)
        {
            //Arrange
            await AuthUtils.AddUsersBearer(httpClient);
            var path = apiPath + "/" + id;

            //Act
            var response = await httpClient.GetAsync(path);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }
        #endregion

        #region Modify tests
        [Fact]
        public async Task Post_ReturnsUnathorized_ForRegularUser()
        {
            //Arrange
            await AuthUtils.AddUsersBearer(httpClient);
            var path = apiPath;

            //Act
            var content = JsonConvert.SerializeObject(new Resource { Title = "Completely normal resource", RuleId = 1 });
            var response = await httpClient.PostAsync(path, new StringContent(content, Encoding.UTF8, this.appJson));

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }
        #endregion


        //#region FolderController.Create
        //[Theory]
        //[InlineData("api/folder")]
        //public async Task CreateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
        //    var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    Assert.Equal("Created", response.ReasonPhrase);
        //}

        //[Theory]
        //[InlineData("api/folder")]
        //public async Task FailedValidationCreateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Fo", IsActive = true });
        //    var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    Assert.Equal("Bad Request", response.ReasonPhrase);
        //}
        //#endregion FolderController.Create



        //#region FolderController.Update
        //[Theory]
        //[InlineData("api/folder/1")]
        //public async Task UpdateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
        //    var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //}

        //[Theory]
        //[InlineData("api/folder/122")]
        //public async Task FailedUpdateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
        //    var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    Assert.Equal("Not Found", response.ReasonPhrase);
        //}

        //[Theory]
        //[InlineData("api/folder/1")]
        //public async Task FailedValidationUpdateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Fo", IsActive = true });
        //    var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    Assert.Equal("Bad Request", response.ReasonPhrase);
        //}

        //[Theory]
        //[InlineData("api/folder/1")]
        //public async Task IsParentInvalidUpdateFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var content = JsonConvert.SerializeObject(new Folder { Title = "Folder 1", ParentFolderId = 3, IsActive = true });
        //    var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

        //    // Assert
        //    Assert.Equal("Forbidden", response.ReasonPhrase);
        //}
        //#endregion FolderController.Update



        //#region FolderController.Delete
        //[Theory]
        //[InlineData("api/folder/2")]
        //public async Task DeleteFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var response = await _client.DeleteAsync(url);

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //}

        //[Theory]
        //[InlineData("api/folder/122")]
        //public async Task FailedDeleteFolderTest(string url)
        //{
        //    await AuthUtils.AddTokenBearerHeader(_client);
        //    var response = await _client.DeleteAsync(url);

        //    // Assert
        //    Assert.Equal("Not Found", response.ReasonPhrase);
        //}
        //#endregion FolderController.Delete


    }
}