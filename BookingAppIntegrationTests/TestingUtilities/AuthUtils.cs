using BookingApp.DTOs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookingAppIntegrationTests.TestingUtilities
{
    public static class AuthUtils
    {
        public static async Task AddTokenBearerHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetToken(client)}");
        }

        static string tokenCache;
        static async Task<string> GetToken(HttpClient client)
        {
            if (tokenCache == null)
            {
                var responseToken = await client.PostAsJsonAsync("/api/auth/login", new
                {
                    Password = "SuperAdmin",
                    Email = "superadmin@admin.cow"
                });

                string tokenJson = responseToken.Content.ReadAsStringAsync().Result;
                AuthTokensDto tokenDto = JsonConvert.DeserializeObject<AuthTokensDto>(tokenJson);

                tokenCache = tokenDto.AccessToken;
            }
            return tokenCache;
        }
    }
}
