using Microsoft.AspNetCore.Mvc.Testing;
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
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

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

    }
}
