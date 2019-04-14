using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using System.Threading.Tasks;
using BookingApp.Services;
using BookingApp.Data.Models;
using BookingApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using BookingApp.DTOs;
using System.Linq;
using BookingApp.Services.Interfaces;
using BookingApp.DTOs.Resource;
using BookingApp.Exceptions;

namespace BookingAppTests.Controllers
{
    public class RuleControllerTest
    {
        #region Initialize
        private Mock<IRuleService> mockServ;
        public RuleControllerTest()
        {
            mockServ = new Mock<IRuleService>();
        }
        #endregion

        #region GetListOfRules
        [Fact]
        public async Task GetListOfRulesForAdmin()
        {
            //arrange
            mockServ.Setup(p => p.GetList()).ReturnsAsync(initRules());
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.IsAdmin).Returns(true);

            //act
            var result = await controller.Object.Rules();

            //Assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var modelOk = Assert.IsAssignableFrom<List<RuleAdminDTO>>(ruleOk.Value);
            Assert.Equal(3, modelOk.Count);
            Assert.Equal("ComputerRule", modelOk.Find(p => p.Title == "ComputerRule").Title);
        }

        [Fact]
        public async Task GetListOfRulesForUser()
        {
            //arrange
            mockServ.Setup(p => p.GetActiveList()).ReturnsAsync(initRules().Where(p => p.IsActive == true));
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.IsAdmin).Returns(false);

            //act
            var result = await controller.Object.Rules();

            //Assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var modelOk = Assert.IsAssignableFrom<List<RuleBasicDTO>>(ruleOk.Value);
            Assert.Equal(2, modelOk.Count);
            Assert.Equal("ComputerRule", modelOk.Find(p => p.Title == "ComputerRule").Title);
        }
        #endregion

        #region GetRule
        [Theory]
        [InlineData(2)]
        public async Task GetRuleForAdmin(int id)
        {
            //arrange
            mockServ.Setup(p => p.Get(id)).ReturnsAsync(initRules().Single(p => p.Id == id));
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.IsAdmin).Returns(true);

            //act
            var result = await controller.Object.GetRule(id);

            //assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var modelOk = Assert.IsType<RuleAdminDTO>(ruleOk.Value);
            Assert.Equal("LibraryRule", modelOk.Title);
            Assert.Equal(20, modelOk.MinTime);
        }

        [Theory]
        [InlineData(2)]
        public async Task GetRuleForUser(int id)
        {
            //arrange
            mockServ.Setup(p => p.Get(id)).ReturnsAsync(initRules().Single(p => p.Id == id));
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.IsAdmin).Returns(false);


            //act
            var result = await controller.Object.GetRule(id);

            //assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var modelOk = Assert.IsType<RuleBasicDTO>(ruleOk.Value);
            Assert.Equal("LibraryRule", modelOk.Title);
            Assert.Equal(20, modelOk.MinTime);
        }

        #endregion

        #region CreateRule
        [Fact]
        public async Task CreateRuleWithInvalidModelReturnsBadRequest()
        {
            //arrange
            mockServ.Setup(p => p.Create(It.IsAny<Rule>()));
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.Object.ModelState.AddModelError("error", "Invalid Rule model");

            //act
            var result = await controller.Object.CreateRule(It.IsAny<RuleDetailedDTO>());

            //assert
            var ruleBad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ruleBad.StatusCode);
        }

        [Fact]
        public async Task CreateRule()
        {
            //arrange
            mockServ.Setup(p => p.Create(someRule()));
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.UserId).Returns(It.IsAny<string>);

            //act
            var result = await controller.Object.CreateRule(someDTORule());

            //assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var ruleModel = Assert.IsType<Rule>(ruleOk.Value);
            Assert.Equal("ComputerRule", ruleModel.Title);
        }
        #endregion

        #region DeleteRule
        [Theory]
        [InlineData(1)]
        public async Task DeleteRule(int id)
        {
            //arrange
            mockServ.Setup(p => p.Delete(id)).Returns(Task.CompletedTask);
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };

            //act
            var result = await controller.Object.DeleteRule(id);

            //assert
            var ruleOk = Assert.IsType<OkResult>(result);
            Assert.Equal(200, ruleOk.StatusCode);

        }

        #endregion

        #region UpdateRule
        [Fact]
        public async Task UpdateRuleWithBadModelReturnsBadRequest()
        {
            //arrange
            mockServ.Setup(f => f.Update(5,someRule())).Returns(Task.CompletedTask);
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.Object.ModelState.AddModelError("error", "Invalid model");
            //act
            var result = await controller.Object.UpdateRule(1, someDTORule());

            //assert
            var ruleBad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ruleBad.StatusCode);
        }

        [Fact]
        public async Task UpdateRule()
        {
            //arrange
            mockServ.Setup(f => f.Update(1, someRule())).Returns(Task.CompletedTask);
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };
            controller.SetupGet(p => p.UserId).Returns(It.IsAny<string>());
            //act
            var result = await controller.Object.UpdateRule(1, someDTORule());

            //Assert
            var ruleOk = Assert.IsType<OkResult>(result);
            Assert.Equal(200, ruleOk.StatusCode);
        }
        #endregion

        #region GetResourcesForRule
        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public async Task GetResourcesForRule(int id)
        {
            //arrange
            var mockResServ = new Mock<IResourcesService>();
            mockResServ.Setup(p => p.ListByRuleKey(id)).ReturnsAsync(resourceForRules());
            var controller = new Mock<RuleController>(mockServ.Object) { CallBase = true };

            //act
            var result = await controller.Object.GetResourcesByRule(id, mockResServ.Object);

            //assert
            var ruleOk = Assert.IsType<OkObjectResult>(result);
            var resourcesOk = Assert.IsAssignableFrom<IEnumerable<ResourceMaxDto>>(ruleOk.Value);
        }
        #endregion

        #region TestDataHelper
        public IEnumerable<Rule> initRules()
        {
            var rules = new List<Rule>();
            rules.Add(new Rule
            {
                Id = 1,
                IsActive = true,
                Title = "ComputerRule",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedUserId = "1",
                UpdatedUserId = "1",
                MinTime = 10,
                MaxTime = 100
            });
            rules.Add(new Rule
            {
                Id = 2,
                IsActive = true,
                Title = "LibraryRule",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedUserId = "2",
                UpdatedUserId = "2",
                MinTime = 20,
                MaxTime = 200
            });
            rules.Add(new Rule
            {
                Id = 3,
                IsActive = false,
                Title = "BunkerRule",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedUserId = "3",
                UpdatedUserId = "3",
                MinTime = 30,
                MaxTime = 300
            });
            return rules;
        }

        public Rule someRule()
        {
            return new Rule
            {
                Id = 1,
                IsActive = true,
                Title = "ComputerRule",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedUserId = "1",
                UpdatedUserId = "1",
                MinTime = 10,
                MaxTime = 100
            };
        }

        public RuleDetailedDTO someDTORule()
        {
            return new RuleDetailedDTO
            {
                IsActive = true,
                Title = "ComputerRule",
                MinTime = 10,
                MaxTime = 100
            };
        }

        public IEnumerable<Resource> resourceForRules()
        {
            return new List<Resource>();
        }
        #endregion

    }
}
