using AutoMapper;
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
using Xunit;

namespace BookingAppTests.Controllers
{
    public class ResourcesControllerTests
    {
        #region List() tests
        [Fact]
        public async void List_ReturnsSomeResources()
        {
            // Arrange 
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.GetList()).ReturnsAsync(TestResources);
            resServiceMock.Setup(service => service.ListActive()).ReturnsAsync(TestResources.Where(r => r.IsActive == true));

            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

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
            bool isAdmin = true;

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.GetList()).ReturnsAsync(TestResources);

            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.List();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ResourceBriefDto>>(okResult.Value);
            Assert.Equal(TestResources.Count(), dtos.Count());
            resServiceMock.Verify(mock => mock.GetList(), Times.Once());
        }

        [Fact]
        public async void List_ReturnsActiveResources()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListActive()).ReturnsAsync(TestResources.Where(r=>r.IsActive == true));

            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.List();

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ResourceBriefDto>>(okResult.Value);
            Assert.True(TestResources.Count() > dtos.Count());
            Assert.Equal(TestResources.Where(r => r.IsActive == true).Count(), dtos.Count());
            resServiceMock.Verify(mock => mock.ListActive(), Times.Once());
        }
        #endregion

        #region Single() tests
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async void Single_ReturnsAllowedResource(bool isActive, bool isAdmin)
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.Get(It.IsAny<int>())).ReturnsAsync(TestResources.First());
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(isActive);

            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.Single(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dto = Assert.IsAssignableFrom<ResourceMaxDto>(okResult.Value);
            resServiceMock.Verify(mock => mock.Get(It.IsAny<int>()), Times.Once());
        }
        #endregion

        #region Create() tests
        [Fact]
        public async void Create_ReturnsCreatedResponse()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());
            resControllerMock.SetupGet(mock => mock.BaseApiUrl).Returns(It.IsAny<string>());

            var fakeResController = resControllerMock.Object;

            IMapper dtoMapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<Resource, ResourceDetailedDto>(); }));
            var dto = dtoMapper.Map<ResourceDetailedDto>(TestResources.First());

            // Act
            var actionResult = await fakeResController.Create(dto);

            //Assert 
            var createdResult = Assert.IsType<CreatedResult>(actionResult);
            Assert.IsAssignableFrom<DateTime>(createdResult.Value.GetType().GetProperty("CreatedTime")?.GetValue(createdResult.Value, null));
            Assert.IsAssignableFrom<int>(createdResult.Value.GetType().GetProperty("ResourceId")?.GetValue(createdResult.Value, null));
            resServiceMock.Verify(mock => mock.Create(It.IsAny<Resource>()), Times.Once());
        }

        [Fact]
        public async void Create_ReturnsBadRequest_OnInvalidModel()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            var bookServiceMock = new Mock<IBookingsService>();

            var subjectResController = new ResourcesController(resServiceMock.Object, bookServiceMock.Object);
            subjectResController.ModelState.AddModelError("blah", "blah");

            // Act
            var actionResult = await subjectResController.Create(It.IsAny<ResourceDetailedDto>());

            //Assert 
            Assert.IsType<BadRequestObjectResult>(actionResult);
            resServiceMock.Verify(mock => mock.Create(It.IsAny<Resource>()), Times.Never());
        }
        #endregion

        #region Update() tests
        [Fact]
        public async void Update_ReturnsCreatedResponse()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());
            resControllerMock.SetupGet(mock => mock.BaseApiUrl).Returns(It.IsAny<string>());

            var fakeResController = resControllerMock.Object;

            IMapper dtoMapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<Resource, ResourceDetailedDto>(); }));
            var dto = dtoMapper.Map<ResourceDetailedDto>(TestResources.First());

            // Act
            var actionResult = await fakeResController.Update(It.IsAny<int>(), dto);

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.IsAssignableFrom<DateTime>(okResult.Value.GetType().GetProperty("UpdatedTime")?.GetValue(okResult.Value, null));
            resServiceMock.Verify(mock => mock.Update(It.IsAny<Resource>()), Times.Once());
        }

        [Fact]
        public async void Update_ReturnsBadRequest_OnInvalidModel()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            var bookServiceMock = new Mock<IBookingsService>();

            var subjectResController = new ResourcesController(resServiceMock.Object, bookServiceMock.Object);
            subjectResController.ModelState.AddModelError("blah", "blah");

            // Act
            var actionResult = await subjectResController.Update(It.IsAny<int>(), It.IsAny<ResourceDetailedDto>());

            //Assert 
            Assert.IsType<BadRequestObjectResult>(actionResult);
            resServiceMock.Verify(mock => mock.Update(It.IsAny<Resource>()), Times.Never());
        }
        #endregion

        #region Delete() tests
        [Fact]
        public async void Delete_ReturnsOkDeletedResponse()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            var bookServiceMock = new Mock<IBookingsService>();

            var subjectResController = new ResourcesController(resServiceMock.Object, bookServiceMock.Object);

            // Act
            var actionResult = await subjectResController.Delete(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.IsAssignableFrom<DateTime>(okResult.Value.GetType().GetProperty("DeletedTime")?.GetValue(okResult.Value, null));
            resServiceMock.Verify(mock => mock.Delete(It.IsAny<int>()), Times.Once());
        }
        #endregion


        #region ListOccupancy() tests
        [Fact]
        public async void ListOccupancy_ReturnsSomeOccupancies()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r=>r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive == true).Select(r => r.Id));

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ReturnsAsync(It.IsAny<double?>());

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
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive == true).Select(r => r.Id));
            
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
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive == true).Select(r => r.Id));

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
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive == true).Select(r => r.Id));

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
        public async void ListOccupancy_ReturnsActiveOccupancies()
        {
            // Arrange
            bool isAdmin = It.IsAny<bool>();

            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.ListKeys()).ReturnsAsync(TestResources.Select(r => r.Id));
            resServiceMock.Setup(service => service.ListActiveKeys()).ReturnsAsync(TestResources.Where(r => r.IsActive == true).Select(r => r.Id));

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
            int expectedCount = TestResources.Where(r => r.IsActive == true).Count();
            Assert.Equal(expectedCount, map.Count());
            resServiceMock.Verify(mock => mock.ListActiveKeys(), Times.Once());
            bookServiceMock.Verify(mock => mock.OccupancyByResource(It.IsAny<int>()), Times.Exactly(expectedCount));
        }
        #endregion

        #region SingleOccupancy() tests
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async void SingleOccupancy_ReturnsAllowedOccupancy(bool isActive, bool isAdmin)
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(isActive);

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.OccupancyByResource(It.IsAny<int>())).ReturnsAsync(It.IsAny<double>());

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.SingleOccupancy(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var occupancyValue = Assert.IsAssignableFrom<double>(okResult.Value);
            bookServiceMock.Verify(mock => mock.OccupancyByResource(It.IsAny<int>()), Times.Once());
        }
        #endregion


        #region ListRelatedBookings() tests
        // ReturnsSomeDTOs_OnAllowedResource
        [Theory]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async void ListRelatedBookings_ReturnsSomeDTOs_OnAllowedResource(bool isActive, bool isAdmin, bool isUser)
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(isActive);

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(TestBookings);

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);
            resControllerMock.SetupGet(mock => mock.IsUser).Returns(isUser);
            resControllerMock.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListRelatedBookings(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<BookingMinimalDTO>>(okResult.Value);
            Assert.NotEmpty(dtos);
            bookServiceMock.Verify(mock => mock.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Theory]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async void ListRelatedBookings_ReturnAdminDTOs(bool isActive, bool isAdmin, bool isUser)
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(isActive);

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(TestBookings);

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);
            resControllerMock.SetupGet(mock => mock.IsUser).Returns(isUser);
            resControllerMock.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListRelatedBookings(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var dtos = Assert.IsAssignableFrom<IEnumerable<BookingAdminDTO>>(okResult.Value);
            Assert.NotEmpty(dtos);
            bookServiceMock.Verify(mock => mock.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void ListRelatedBookings_ReturnOwnerDTOs_OnOwnedBookings()
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(true);

            var bookServiceMock = new Mock<IBookingsService>();
            bookServiceMock.Setup(service => service.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(TestBookings);

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(false);
            resControllerMock.SetupGet(mock => mock.IsUser).Returns(true);
            resControllerMock.SetupGet(mock => mock.UserId).Returns(this.soleBookingCreator);

            var fakeResController = resControllerMock.Object;

            // Act
            var actionResult = await fakeResController.ListRelatedBookings(It.IsAny<int>());

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var minimalDtos = Assert.IsAssignableFrom<IEnumerable<BookingMinimalDTO>>(okResult.Value);
            var dtos = Assert.IsAssignableFrom<IEnumerable<BookingOwnerDTO>>(minimalDtos.Cast<BookingOwnerDTO>());
            Assert.NotEmpty(dtos);
            bookServiceMock.Verify(mock => mock.ListBookingOfResource(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }
        #endregion

        #region AuthorizeForSingleResource() [helper] test
        [Theory]
        [InlineData(false, false)]
        public async void AuthorizeForSingleResourceHelper_ThrowsNotFound(bool isActive, bool isAdmin)
        {
            // Arrange
            var resServiceMock = new Mock<IResourcesService>();
            resServiceMock.Setup(service => service.IsActive(It.IsAny<int>())).ReturnsAsync(isActive);

            var bookServiceMock = new Mock<IBookingsService>();

            var resControllerMock = new Mock<ResourcesController>(resServiceMock.Object, bookServiceMock.Object) { CallBase = true };
            resControllerMock.SetupGet(mock => mock.IsAdmin).Returns(isAdmin);

            var fakeResController = resControllerMock.Object;

            //Assert-Act
            await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => fakeResController.AuthorizeForSingleResource(It.IsAny<int>()));

            //Assert
            resServiceMock.Verify(mock => mock.IsActive(It.IsAny<int>()), Times.Once());
        }
        #endregion

        #region Utilities
        IEnumerable<Resource> testResources;
        IEnumerable<Resource> TestResources => testResources ?? FormTestResources();
        IEnumerable<Resource> FormTestResources()
        {
            testResources = new [] 
            {
                  new Resource() { Id = 1, Title = "Nothern View", IsActive = true, RuleId = 1 },
                  new Resource() { Id = 2, Title = "Southern View", IsActive = true },
                  new Resource() { Id = 3, Title = "Flag", IsActive = true },

                  new Resource() { Id = 4, Title = "Trumpet Ensemble", IsActive = true },

                  new Resource() { Id = 5, Title = "First Floor Hallway", IsActive = true },

                  new Resource() { Id = 6, Title = "Natural Museum", IsActive = false },
                  new Resource() { Id = 7, Title = "Art Museum", IsActive = false },
                  new Resource() { Id = 8, Title = "History Museum", IsActive = true },

                  new Resource() { Id = 9, Title = "Civil Defence Alarm", IsActive = true },

                  new Resource() { Id = 10, Title = "Cruiser Bicycle #2000", IsActive = true },
                  new Resource() { Id = 11, Title = "Cruiser Bicycle #46", IsActive = true },
                  new Resource() { Id = 12, Title = "Ukraine Tier0 Bicycle", IsActive = true },
                  new Resource() { Id = 13, Title = "Mountain Bike Roger", IsActive = false },
            };
            return testResources;
        }

        string soleBookingCreator = "SoleBookingCreator";
        IEnumerable<Booking> testBookings;
        IEnumerable<Booking> TestBookings => testBookings ?? FormTestBookings();
        IEnumerable<Booking> FormTestBookings()
        { 
            var sampleSize = 5;
            var newBookings = new Booking[sampleSize];
            var now = DateTime.Now;

            for (int i = 1; i <= sampleSize; i++)
            {
                var curBook = new Booking
                {
                    Id = i,
                    Note = "Note_" + i,
                    ResourceId = 1,
                    CreatedUserId = soleBookingCreator,
                    UpdatedUserId = "Updater_" + i
                };
                curBook.CreatedTime = curBook.StartTime = curBook.EndTime = curBook.UpdatedTime = now;
                newBookings[i-1] = curBook;
            }

            testBookings = newBookings;
            return testBookings;
        }
        #endregion
    }
}