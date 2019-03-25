using BookingApp.Exceptions;
using BookingApp.Repositories;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TestingUtilities;
using Xunit;

namespace BookingAppTests.Services
{
    public class ResourcesServiceTests
    {
        [Fact]
        public async void GetOccupanciesByIDs_ReturnsCorrectAndSwallowsDisjointBookingsExceptions()
        {
            foreach (Exception ex in new Exception[] { new KeyNotFoundException(), new FieldValueAbsurdException()})
            {
                //Arrange
                var resRepoMock = new Mock<IResourcesRepository>();
                var bookRepoMock = new Mock<IBookingsRepository>();

                var resServiceMock = new Mock<ResourcesService>(resRepoMock.Object, bookRepoMock.Object) { CallBase = true };
                resServiceMock.Setup(mock => mock.OccupancyByResource(It.IsAny<int>())).ThrowsAsync(ex);

                //Act
                var result = await resServiceMock.Object.GetOccupanciesByIDs(ResourceUtils.TestSet.Select(r => r.Id));

                //Assert
                Assert.IsAssignableFrom<Dictionary<int, double?>>(result);
            }
        }

        [Fact]
        public async void GetOccupanciesByIDs_ThrowsUnhandledBookingExceptions()
        {
            //Arrange
            var resRepoMock = new Mock<IResourcesRepository>();
            var bookRepoMock = new Mock<IBookingsRepository>();

            var resServiceMock = new Mock<ResourcesService>(resRepoMock.Object, bookRepoMock.Object) { CallBase = true };
            resServiceMock.Setup(mock => mock.OccupancyByResource(It.IsAny<int>())).ThrowsAsync(new Exception());

            //Assert-Act
            var ex = await Assert.ThrowsAsync<Exception>(() => resServiceMock.Object.GetOccupanciesByIDs(ResourceUtils.TestSet.Select(r => r.Id)));

            //Assert
            Assert.IsNotType<KeyNotFoundException>(ex);
            Assert.IsNotType<FieldValueAbsurdException>(ex);
        }
    }
}