using BookingApp.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace BookingAppIntegrationTests.Scenarios
{
    public class FolderControllerTest : IClassFixture<WebApplicationFactory<BookingApp.Startup>>
    {

        private readonly WebApplicationFactory<BookingApp.Startup> _factory;
        
        public FolderControllerTest(WebApplicationFactory<BookingApp.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/folder")]
        public async Task GetFolders(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var stringResponse = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<IEnumerable<Folder>>(stringResponse);
            Assert.Contains(folders, p => p.Title == "Town Hall");
 
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder/1")]
        public async Task GetFolderById(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await client.GetAsync(url);

            // Assert
            var folder = JsonConvert.DeserializeObject<Folder>(await response.Content.ReadAsStringAsync());
            Assert.Contains(folder.Title, "Town Hall");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/folder/122")]
        public async Task GetFaildedFolderById(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
            var response = await client.GetAsync(url);

            // Assert

            var statusCod = response.StatusCode.ToString();
            Assert.Equal("NotFound", statusCod);
        }

        //[Theory]
        //[InlineData("/api/folder/5")]
        //public async Task GetDeleteFolder(string url)
        //{
        //    // Arrange
        //    var client = _factory.CreateClient();

        //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await this.getToken()}");
        //    var response = await client.DeleteAsync(url);

        //    // Assert

        //    var statusCod = response.StatusCode.ToString();
        //    Assert.Equal("Ok", statusCod);
        //    response.EnsureSuccessStatusCode();
        //}



        #region Utility
        private async Task<string> getToken()
        {
            var client = _factory.CreateClient();
            var responseToken = await client.PostAsJsonAsync("/api/auth/login", new
            {
                Password = "SuperAdmin",
                Email = "superadmin@admin.cow"
            });
            return (((responseToken.Content.ReadAsStringAsync().Result).Substring(16)).Split(',')[0]).Trim('"');
        }
        #endregion Utility
    }
}
