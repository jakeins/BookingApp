using BookingApp.Controllers;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingAppTests.Controllers
{
    public class ResourcesControllerTest
    {
        #region List() tests
        [Fact]
        public async void List_ReturnsOkWithAListOfSomeResources_ForEveryone()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.GetList()).ReturnsAsync(TestResources);
            resServiceMock.Setup(service => service.ListActive()).ReturnsAsync(TestResources.Where(r => r.IsActive != false));
            var fakeResController = ProduceFakeResourcesController(resServiceMock, It.IsAny<bool>());

            // Act
            var actionResult = await fakeResController.List();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ResourceBriefDto>>(okResult.Value);
            Assert.NotEmpty(dtos);
            resServiceMock.Verify(mock => mock.GetList(), Times.AtMostOnce());
            resServiceMock.Verify(mock => mock.ListActive(), Times.AtMostOnce());
        }

        [Fact]
        public async void List_ReturnsAllResources_ForAdmin()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.GetList()).ReturnsAsync(TestResources);
            var fakeResController = ProduceFakeResourcesController(resServiceMock, isAdmin: true);

            // Act
            var actionResult = await fakeResController.List();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ResourceBriefDto>>(okResult.Value);
            Assert.Equal(TestResources.Count(), dtos.Count());
            resServiceMock.Verify(mock => mock.GetList(), Times.Once());
        }

        [Fact]
        public async void List_ReturnsActiveResources_ForUser()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListActive()).ReturnsAsync(TestResources.Where(r=>r.IsActive != false));
            var fakeResController = ProduceFakeResourcesController(resServiceMock, isAdmin: false);

            // Act
            var actionResult = await fakeResController.List();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ResourceBriefDto>>(okResult.Value);
            Assert.True(TestResources.Count() > dtos.Count());
            Assert.Equal(TestResources.Where(r => r.IsActive != false).Count(), dtos.Count());
            resServiceMock.Verify(mock => mock.ListActive(), Times.Once());
        }
        #endregion

        #region Single() tests

        #endregion

        #region Create() tests

        #endregion

        #region Update() tests

        #endregion

        #region Delete() tests

        #endregion


        #region ListOccupancy() tests
        [Fact]
        public async void ListOccupancy_ReturnsOkWithAListOfSomeOccupancies_ForEveryone()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r=>r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive != false).Select(r => r.Id));

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ReturnsAsync(It.IsAny<int>());

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListOccupancy();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var map = Assert.IsAssignableFrom<Dictionary<int, double?>>(okResult.Value);
            Assert.NotEmpty(map);
            resServiceMock.Verify(mock => mock.ListKeys(), Times.AtMostOnce());
            resServiceMock.Verify(mock => mock.ListActiveKeys(), Times.AtMostOnce());
        }

        [Fact]
        public async void ListOccupancy_SwallowsDisjointBookingsExceptions()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r => r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive != false).Select(r => r.Id));
            
            foreach (Exception ex in new Exception[] { new KeyNotFoundException(), new FieldValueAbsurdException()})
            {
                //Arrange (continued)
                var bookServiceMock = new Mock<IBookingsService>();
                bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ThrowsAsync(ex);

                var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
                resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

                var fakeResController = resControllerMock.Object;

                //Act
                await fakeResController.ListOccupancy();
            }
        }

        [Fact]
        public async void ListOccupancy_ThrowsUnhandledBookingExceptions()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r => r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive != false).Select(r => r.Id));

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ThrowsAsync(new Exception());

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            //Act
            var ex = await Assert.ThrowsAsync<Exception>(() => fakeResController.ListOccupancy());

            //Assert
            Assert.IsNotType<KeyNotFoundException>(ex);
            Assert.IsNotType<FieldValueAbsurdException>(ex);
        }

        [Fact]
        public async void ListOccupancy_ReturnsAllOccupancies_ForAdmin()
        {
            // Arrange
            bool isAdmin = true;

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r => r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive != false).Select(r => r.Id));

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ReturnsAsync(It.IsAny<int>());

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListOccupancy();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var map = Assert.IsAssignableFrom<Dictionary<int, double?>>(okResult.Value);
            int expectedCount = TestResources.Count();
            Assert.Equal(expectedCount, map.Count());
            resServiceMock.Verify(mock => mock.ListKeys(), Times.Once());
            bookServiceMock.Verify(mock => mock.OccupancyByResource(It.IsAny<int>()), Times.Exactly(expectedCount));
        }

        [Fact]
        public async void ListOccupancy_ReturnsActiveOccupancies_ForUser()
        {
            // Arrange
            bool isAdmin = false;

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r => r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive != false).Select(r => r.Id));

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ReturnsAsync(It.IsAny<int>());

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListOccupancy();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var map = Assert.IsAssignableFrom<Dictionary<int, double?>>(okResult.Value);
            int expectedCount = TestResources.Where(r => r.IsActive != false).Count();
            Assert.Equal(expectedCount, map.Count());
            resServiceMock.Verify(mock => mock.ListActiveKeys(), Times.Once());
            bookServiceMock.Verify(mock => mock.OccupancyByResource(It.IsAny<int>()), Times.Exactly(expectedCount));
        }

        #endregion

        #region SingleOccupancy() tests

        #endregion


        #region ListRelatedBookings() tests

        #endregion


        #region Utilities

        ResourcesController ProduceFakeResourcesController(Mock<IResourcesService> resServiceMock, bool isAdmin = false)
        {
            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            return resControllerMock.Object;
        }

        IEnumerable<Resource> testResources;
        IEnumerable<Resource> TestResources => testResources ?? FormTestResources();
        IEnumerable<Resource> FormTestResources()
        {
            //var resources = new Dictionary<int, Resource> {
            //    {  1, new Resource() { Title = "Nothern View",          FolderId = 2,   RuleId = 1 } },
            //    {  2, new Resource() { Title = "Southern View",         FolderId = 2,   RuleId = 2 } },
            //    {  3, new Resource() { Title = "Flag",                  FolderId = 2,   RuleId = 1 } },

            //    {  4, new Resource() { Title = "Trumpet Ensemble",      FolderId = 3,   RuleId = 3 } },

            //    {  5, new Resource() { Title = "First Floor Hallway",   FolderId = 1,   RuleId = 2 } },

            //    {  6, new Resource() { Title = "Natural Museum",        FolderId = 4,   RuleId = 4, IsActive = false } },
            //    {  7, new Resource() { Title = "Art Museum",            FolderId = 4,   RuleId = 4, IsActive = false } },
            //    {  8, new Resource() { Title = "History Museum",        FolderId = 4,   RuleId = 4 } },

            //    {  9, new Resource() { Title = "Civil Defence Alarm",                   RuleId = 1 } },

            //    { 10, new Resource() { Title = "Cruiser Bicycle #2000", FolderId = 5,   RuleId = 5 } },
            //    { 11, new Resource() { Title = "Cruiser Bicycle #46",   FolderId = 5,   RuleId = 3 } },
            //    { 12, new Resource() { Title = "Ukraine Tier0 Bicycle", FolderId = 5,   RuleId = 2 } },
            //    { 13, new Resource() { Title = "Mountain Bike Roger",   FolderId = 5,   RuleId = 3, IsActive = false } },
            //};
            //testResources = resources.Select(v => v.Value);
            

            testResources = new [] {
                  new Resource() { Id = 1, Title = "Nothern View"},
                  new Resource() { Id = 2, Title = "Southern View"},
                  new Resource() { Id = 3, Title = "Flag"},

                  new Resource() { Id = 4, Title = "Trumpet Ensemble"},

                  new Resource() { Id = 5, Title = "First Floor Hallway"},

                  new Resource() { Id = 6, Title = "Natural Museum", IsActive = false },
                  new Resource() { Id = 7, Title = "Art Museum", IsActive = false },
                  new Resource() { Id = 8, Title = "History Museum" },

                  new Resource() { Id = 9, Title = "Civil Defence Alarm"},

                  new Resource() { Id = 10, Title = "Cruiser Bicycle #2000"},
                  new Resource() { Id = 11, Title = "Cruiser Bicycle #46" },
                  new Resource() { Id = 12, Title = "Ukraine Tier0 Bicycle"},
                  new Resource() { Id = 13, Title = "Mountain Bike Roger", IsActive = false },
            };


            return testResources;
        }
        #endregion
    }
}