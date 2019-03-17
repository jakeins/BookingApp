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
        [InlineData("/api/folder")]
        public async Task GetFoldersAllTest(string url)
        {
            var response = await _client.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<List<Folder>>(stringResponse);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.Equal(1, folders.Count);
        }

        [Theory]
        [InlineData("/api/folder")]
        public async Task GetFoldersIsNotActiveTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);
            
            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<List<Folder>>(stringResponse);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.Equal(4, folders.Count);
        }
        #endregion FolderController.Index

        #region FolderController.Detail
        [Theory]
        [InlineData("/api/folder/1")]
        public async Task GetFolderByIdTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);

            var folder = JsonConvert.DeserializeObject<Folder>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Contains(folder.Title, "Town Hall");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder/122")]
        public async Task GetFaildedFolderByIdTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);

            // Assert
            var statusCod = response.StatusCode.ToString();
            Assert.Equal("NotFound", statusCod);
        }
        #endregion FolderController.Detail

        #region FolderController.Create
        [Theory]
        [InlineData("/api/folder")]
        public async Task CreateFolderTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");

            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Created", response.StatusCode.ToString());
        }

        [Theory]
        [InlineData("/api/folder")]
        public async Task FailedValidationCreateFolderTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");

            var content = JsonConvert.SerializeObject(new Folder { Title = "Fo", IsActive = true });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("BadRequest", response.StatusCode.ToString());
        }
        #endregion FolderController.Create

        #region FolderController.Delete
        [Theory]
        [InlineData("/api/folder/2")]
        public async Task DeleteFolderTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/folder/122")]
        public async Task FailedDeleteFolderTest(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.DeleteAsync(url);

            // Assert
            var statusCod = response.StatusCode.ToString();
            Assert.Equal("NotFound", statusCod);
        }
        #endregion FolderController.Delete


        #region Utility
        private async Task<string> getToken()
        {
            var responseToken = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                Password = "SuperAdmin",
                Email = "superadmin@admin.cow"
            });
            return (((responseToken.Content.ReadAsStringAsync().Result).Substring(16)).Split(',')[0]).Trim('"');
        }
        #endregion Utility
    }
}
