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

        [Theory]
        [InlineData("/api/folder")]
        public async Task GetFoldersAll(string url)
        {
            var response = await _client.GetAsync(url);

            // Assert
            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<IEnumerable<Folder>>(stringResponse);
 
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder")]
        public async Task GetFoldersIsNotActive(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);

            // Assert
            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<IEnumerable<Folder>>(stringResponse);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder/1")]
        public async Task GetFolderById(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);

            // Assert
            var folder = JsonConvert.DeserializeObject<Folder>(await response.Content.ReadAsStringAsync());
            Assert.Contains(folder.Title, "Folder 1");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder/122")]
        public async Task GetFaildedFolderById(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.GetAsync(url);

            // Assert
            var statusCod = response.StatusCode.ToString();
            Assert.Equal("NotFound", statusCod);
        }

        [Theory]
        [InlineData("/api/folder")]
        public async Task CreateFolder(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");

            var content = JsonConvert.SerializeObject(new Folder { Title = "Folder new", IsActive = true });
            var response = await _client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Created", response.StatusCode.ToString());
        }


        [Theory]
        [InlineData("/api/folder/2")]
        public async Task DeleteFolder(string url)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await _client.DeleteAsync(url);

            // Assert

            var statusCod = response.StatusCode.ToString();
            Assert.Equal("OK", statusCod);
            response.EnsureSuccessStatusCode();
        }



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
