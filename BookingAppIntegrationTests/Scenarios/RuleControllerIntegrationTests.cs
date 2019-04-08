using BookingApp;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingAppIntegrationTests.TestingUtilities;
using BookingAppIntegrationTests.Tests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static BookingAppIntegrationTests.TestingUtilities.AuthUtils;

namespace BookingAppIntegrationTests.Scenarios
{
    public class RuleControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        readonly HttpClient _httpClient;

        public RuleControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        #region GetRules
        [Theory]
        [InlineData("api/rules")]
        public async Task GetRulesForAdmin(string path)
        {
            //arrange

            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var response = await _httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var rules = JsonConvert.DeserializeObject<List<RuleAdminDTO>>(stringResponse);

            //assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(rules.Count != 0);
        }

        [Theory]
        [InlineData("api/rules")]
        public async Task GetRulesForUser(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveUser);

            //act
            var response = await _httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var rules = JsonConvert.DeserializeObject<List<RuleBasicDTO>>(stringResponse);

            //assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(5, rules.Count);
        }
        #endregion
        #region GetRuleDetailed
        [Theory]
        [InlineData("api/rules/6")]
        public async Task GetRuleForAdmin(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var response = await _httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var rule = JsonConvert.DeserializeObject<RuleAdminDTO>(stringResponse);

            //assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Inactive Rule", rule.Title);
        }


        [Theory]
        [InlineData("api/rules/122")]
        public async Task GetRuleForAdminReturnBadRequest(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var response = await _httpClient.GetAsync(path);

            //assert
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/rules/2")]
        public async Task GetRuleForUser(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveUser);

            //act
            var response = await _httpClient.GetAsync(path);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var rule = JsonConvert.DeserializeObject<RuleBasicDTO>(stringResponse);

            //assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Rooms", rule.Title);
        }

        [Theory]
        [InlineData("api/rules/29")]
        public async Task GetRuleForUserReturnsBadRequest(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveUser);

            //act
            var response = await _httpClient.GetAsync(path);

            //assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/rules/6")]
        public async Task GetInactiveRuleForUserReturnsBadRequest(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveUser);

            //act
            var response = await _httpClient.GetAsync(path);

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion
        #region CreateRule
        [Theory]
        [InlineData("api/rules")]
        public async Task CreateRule(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var serialized = JsonConvert.SerializeObject(SomeRule());
            var response = await _httpClient.PostAsync(path, new StringContent(serialized, Encoding.UTF8, "application/json"));

            //assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/rules")]
        public async Task CreateRuleWithBadModelReturnsError(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var serialized = JsonConvert.SerializeObject(SomeBadRule());
            var response = await _httpClient.PostAsync(path, new StringContent(serialized, Encoding.UTF8, "application/json"));

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region UpdateRule
        [Theory]
        [InlineData("api/rules/6")]
        public async Task UpdateRule(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var serialized = JsonConvert.SerializeObject(SomeRule());
            var response = await _httpClient.PutAsync(path, new StringContent(serialized, Encoding.UTF8, "application/json"));

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Theory]
        [InlineData("api/rules/121")]
        public async Task UpdateRuleWithBadIdReturnsError(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var serialized = JsonConvert.SerializeObject(SomeRule());
            var response = await _httpClient.PutAsync(path, new StringContent(serialized, Encoding.UTF8, "application/json"));

            //assert
            Assert.Equal("Not Found", response.ReasonPhrase);
        }

        [Theory]
        [InlineData("api/rules/6")]
        public async Task UpdateRuleWithBadModelReturnsError(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var serialized = JsonConvert.SerializeObject(SomeBadRule());
            var response = await _httpClient.PutAsync(path, new StringContent(serialized, Encoding.UTF8, "application/json"));

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region DeleteRule
        [Theory]
        [InlineData("api/rules/5")]
        public async Task DeleteRule(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var response = await _httpClient.DeleteAsync(path);

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/rules/568")]
        public async Task DeleteRuleWithBadIdReturnsError(string path)
        {
            //arrange
            _httpClient.AddBearerFor(UserType.ActiveAdmin);

            //act
            var response = await _httpClient.DeleteAsync(path);

            //assert
            Assert.Equal("Not Found", response.ReasonPhrase);
        }
        #endregion


        #region dataUtils
        private Rule SomeRule()
        {
            var rule = new Rule
            {
                Title = "Library Rule",
                MinTime = 10,
                MaxTime = 100,
                StepTime = 0,
                ServiceTime = 0,
                PreOrderTimeLimit = 0,
                ReuseTimeout = 0,
                IsActive = false
            };
            return rule;
        }

        private Rule SomeBadRule()
        {
            var rule = new Rule
            {
                Title = "Library Rule",
                StepTime = 0,
                ServiceTime = 0,
                PreOrderTimeLimit = 0,
                ReuseTimeout = 0,
            };
            return rule;
        }
        #endregion
    }
}
