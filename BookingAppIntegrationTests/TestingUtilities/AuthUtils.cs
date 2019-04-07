using BookingApp.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookingAppIntegrationTests.TestingUtilities
{
    public static class AuthUtils
    {
        public static void AddBearerForAdmin(this HttpClient client) => AddBearerFor(client, UserType.ActiveAdmin);
        public static void AddBearerForUser(this HttpClient client) => AddBearerFor(client, UserType.ActiveUser);
        public static void AddBearerFor(this HttpClient client, UserType type) => AddBearer(client, GetTestToken(client, type));
        public static async Task AddBearerFor(this HttpClient client, Login login) => AddBearer(client, await TestToken.ProduceInstance(client, login));

        static void AddBearer(this HttpClient client, TestToken token)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer { token.AuthToken.AccessToken }");
        }

        static readonly Dictionary<UserType, Login> logins = new Dictionary<UserType, Login>()
        {
            { UserType.ActiveUser, new Login("cheetah@user.cow", "Cheetah")},
            { UserType.ActiveAdmin, new Login("superadmin@admin.cow", "SuperAdmin")},
            { UserType.DeletableUser, new Login("tiger@admin.cow", "Tiger")},
            { UserType.UpdatableUser, new Login("lion@user.cow", "Lion")}
        };
        static Dictionary<HttpClient, Dictionary<UserType, TestToken>> tokenCache = new Dictionary<HttpClient, Dictionary<UserType, TestToken>>();

        public static TestToken GetTestToken(this HttpClient client, UserType type)
        {
            lock (tokenCache)
            {
                InitializeTokens(client);
                return tokenCache[client][type];
            }
        }

        static void InitializeTokens(HttpClient client)
        {
            if (!tokenCache.ContainsKey(client))
            {
                var newClientEntry = tokenCache[client] = new Dictionary<UserType, TestToken>();
                foreach (var entry in logins)
                    newClientEntry[entry.Key] = TestToken.ProduceInstance(client, entry.Value).Result;
            }
        }

        public enum UserType
        {
            ActiveUser,
            ActiveAdmin,
            DeletableUser,
            UpdatableUser
        }

        public struct Login
        {
            public string Email { get; set; }
            public string Password { get; set; }

            public Login(string email, string password)
            {
                Email = email;
                Password = password;
            }
        }

        public class TestToken
        {
            public AuthTokensDto AuthToken;
            public JwtSecurityToken JwtToken;
            public string UserID;
            public string UserName;
            public string UserEmail;

            public static async Task<TestToken> ProduceInstance(HttpClient client, Login login)
            {
                var responseToken = await client.PostAsJsonAsync("/api/auth/login", new
                {
                    login.Email,
                    login.Password
                });

                string response = responseToken.Content.ReadAsStringAsync().Result;
                AuthTokensDto authToken = JsonConvert.DeserializeObject<AuthTokensDto>(response);
                var tempJwt = new JwtSecurityToken(authToken.AccessToken);

                return new TestToken
                {
                    AuthToken = authToken,
                    JwtToken = tempJwt,
                    UserID = tempJwt.Claims.First(c => c.Type == "uid").Value,
                    UserName = tempJwt.Claims.First(c => c.Type == "sub").Value,
                    UserEmail = tempJwt.Claims.First(c => c.Type == "email").Value
                };
            }
        }
    }
}
