using BookingApp;
using BookingApp.Data.Models;
using BookingAppIntegrationTests.Tests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookingAppIntegrationTests.Scenarios
{
    public class FolderControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        
        public FolderControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        #region FolderController.Index
        [Theory]
        [InlineData("api/folder")]
        public async Task GetFoldersIsAdminTest(string url)
        {
            await this.AddTokenBearerHeader();
            var response = await _client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<List<Folder>>(stringResponse);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.Equal(5, folders.Count);
        }


        #endregion FolderController.Index

        #region FolderController.Detail
        [Theory]
        [InlineData("api/folder/1")]
        public async Task GetFolderByIdTest(string url)
        {
            await this.AddTokenBearerHeader();
            var response = await _client.GetAsync(url);
            var folder = JsonConvert.DeserializeObject<Folder>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Contains(folder.Title, "Town Hall");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("api/folder/122")]
        public async Task GetFaildedFolderByIdTest(string url)
        {
            await this.AddTokenBearerHeader();
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal("Not Found", response.ReasonPhrase);
        }
        #endregion FolderController.Detail

        #region FolderController.Create
        [Theory]
        [InlineData("api/folder")]
        public async Task CreateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Created", response.ReasonPhrase);
        }

        [Theory]
        [InlineData("api/folder")]
        public async Task FailedValidationCreateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Fo", IsActive = true });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("Bad Request", response.ReasonPhrase);
        }
        #endregion FolderController.Create

        #region FolderController.Update
        [Theory]
        [InlineData("api/folder/1")]
        public async Task UpdateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("api/folder/122")]
        public async Task FailedUpdateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("Not Found", response.ReasonPhrase);
        }

        [Theory]
        [InlineData("api/folder/1")]
        public async Task FailedValidationUpdateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Fo", IsActive = true });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("Bad Request", response.ReasonPhrase);
        }

        [Theory]
        [InlineData("api/folder/1")]
        public async Task IsParentInvalidUpdateFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder 1", ParentFolderId = 3, IsActive = true });
            var response = await _client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("Forbidden", response.ReasonPhrase);
        }
        #endregion FolderController.Update

        #region FolderController.Delete
        [Theory]
        [InlineData("api/folder/2")]
        public async Task DeleteFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var response = await _client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("api/folder/122")]
        public async Task FailedDeleteFolderTest(string url)
        {
            await this.AddTokenBearerHeader();
            var response = await _client.DeleteAsync(url);

            // Assert
            Assert.Equal("Not Found", response.ReasonPhrase);
        }
        #endregion FolderController.Delete


        #region Utility
        private async Task AddTokenBearerHeader()
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.GetToken()}");
        }

        private string tokenCache;
        private async Task<string> GetToken()
        {
            if (tokenCache == null)
            {
                var responseToken = await _client.PostAsJsonAsync("/api/auth/login", new
                {
                    Password = "SuperAdmin",
                    Email = "superadmin@admin.cow"
                });

                var tokenJson = responseToken.Content.ReadAsStringAsync().Result;
                var tokenMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenJson);

                tokenCache = tokenMap["accessToken"];
            }
            return tokenCache;
        }
        #endregion Utility
    }
}