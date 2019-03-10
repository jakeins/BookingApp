using BookingApp.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
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

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJTdXBlckFkbWluIiwiZW1haWwiOiJzdXBlcmFkbWluQGFkbWluLmNvdyIsImp0aSI6IjZjOTI4MzY0LWNkMTAtNDE5NC05ZTQzLTNhNmYwMDZjNDk0YSIsInVpZCI6IjkyZjA0Y2U3LTkwZDItNGNiMy05NmMwLWViOWE0MTNhMzA2ZCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJBZG1pbiIsIlVzZXIiXSwiZXhwIjoxNTUyMjI2MzMxLCJpc3MiOiJCb29raW5nQXBwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdCJ9.-L8NN1-t-8-9Azqoxxe6Soxd5PZPeMNnTFRPqCANuVY");
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }



    }
}
