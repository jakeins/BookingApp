using BookingApp.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BookingAppIntegrationTests.TestingUtilities
{
    public static class AuthUtils
    {
        static Dictionary<HttpClient, string> HttpClients = new Dictionary<HttpClient, string>();
        static List<string> AddBearerForCalls = new List<string>();
        static List<string> GetTestTokenCalls = new List<string>();

        public static void AddBearerFor(this HttpClient httpClient, UserType userType, [CallerMemberName] string callerName = "")
        {
            lock(AddBearerForCalls)
                AddBearerForCalls.Add(callerName);

            AddBearer(httpClient, GetTestToken(httpClient, userType, callerName));
        }

        static void AddBearer(this HttpClient httpClient, TestToken token)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token.AuthToken.AccessToken }");
        }

        static readonly Dictionary<UserType, Login> logins = new Dictionary<UserType, Login>()
        {
            { UserType.ActiveUser, new Login("cheetah@user.cow", "Cheetah")},
            { UserType.ActiveAdmin, new Login("superadmin@admin.cow", "SuperAdmin")},
            { UserType.DeletableUser, new Login("tiger@admin.cow", "Tiger")},
            { UserType.UpdatableUser, new Login("lion@user.cow", "Lion")}
        };
        static Dictionary<HttpClient, Dictionary<UserType, TestToken>> tokenCache = new Dictionary<HttpClient, Dictionary<UserType, TestToken>>();

        public static TestToken GetTestToken(this HttpClient httpClient, UserType userType, [CallerMemberName] string callerName = "")
        {
            lock (AddBearerForCalls)
                GetTestTokenCalls.Add(callerName);

            lock (tokenCache)
            {
                if (!tokenCache.ContainsKey(httpClient))
                {
                    lock (HttpClients)
                        HttpClients.Add(httpClient, callerName);
                    InitializeTokens(httpClient);
                }
                return tokenCache[httpClient][userType];
            }
        }

        static void InitializeTokens(HttpClient httpClient)
        {
            var newClientEntry = tokenCache[httpClient] = new Dictionary<UserType, TestToken>();

            foreach (var entry in logins)
            {
                try
                {
                    newClientEntry[entry.Key] = TestToken.ProduceInstance(httpClient, entry.Value).Result;
                }
                catch (AggregateException ex) when (entry.Key == UserType.DeletableUser && ex.InnerException is ApplicationException)
                {
                }
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

            public static async Task<TestToken> ProduceInstance(HttpClient httpClient, Login login)
            {
                var responseToken = await httpClient.PostAsJsonAsync("/api/auth/login", new
                {
                    login.Email,
                    login.Password
                });

                string response = responseToken.Content.ReadAsStringAsync().Result;

                if (!responseToken.IsSuccessStatusCode)
                    throw new ApplicationException(responseToken.ReasonPhrase);

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
