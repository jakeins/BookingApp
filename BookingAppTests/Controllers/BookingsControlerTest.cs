using BookingApp.Controllers;
using BookingApp.Data.Models;
using BookingApp.DTOs;
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
    public class BookingsControllerTest
    {
        #region Booking details

        [Fact]
        public async Task GetDetailsAdmin()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync((await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First()
                    );

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(true);

            var result = await mockBookingsControler.Object.Details(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<BookingAdminDTO>(okResult.Value);

            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetDetailsOwner()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };
            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(booking.CreatedUserId);

            var result = await mockBookingsControler.Object.Details(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<BookingOwnerDTO>(okResult.Value);

            Assert.Equal(booking.Id, model.Id);
        }

        [Fact]
        public async Task GetDetailsOther()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };
            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(It.IsNotIn(booking.CreatedUserId));

            var result = await mockBookingsControler.Object.Details(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<BookingMinimalDTO>(okResult.Value);

            Assert.Equal(booking.ResourceId, model.ResourceID);
        }

        #endregion Booking details

        #region Update

        [Fact]
        public async Task UpdateOtherBookingGenException()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns((string)null);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            await Assert.ThrowsAsync<BookingApp.Exceptions.OperationRestrictedException>(
                async () => await mockBookingsControler.Object.Update(1, bookingUpdateDTO));

            mockBookingsService.Verify(
                mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task UpdateOwnedBooking()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(booking.CreatedUserId);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Update(1, bookingUpdateDTO);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null), Times.Once());
        }

        [Fact]
        public async Task UpdateAdminBooking()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(true);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Update(1, bookingUpdateDTO);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Update(It.IsAny<int>(), null, null, It.IsAny<string>(), null), Times.Once());
        }

        #endregion Update

        #region Delete

        [Fact]
        public async Task DeleteAdmin()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Delete(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(true);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Delete(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task DeleteOwnedBooking()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Delete(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(booking.CreatedUserId);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Delete(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task DeleteOtherBookingGenException()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Delete(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns((string)null);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            await Assert.ThrowsAsync<BookingApp.Exceptions.OperationRestrictedException>(
                async () => await mockBookingsControler.Object.Delete(1));

            mockBookingsService.Verify(
                mock => mock.Delete(It.IsAny<int>()), Times.Never());
        }

        #endregion Delete

        #region Delete

        [Fact]
        public async Task TerminateAdmin()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(true);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Terminate(1);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task TerminateOwnedBooking()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns(booking.CreatedUserId);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            var result = await mockBookingsControler.Object.Terminate(1);
            var okResult = Assert.IsType<OkObjectResult>(result);

            mockBookingsService.Verify(
                mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task TerminateOtherBookingGenException()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            Booking booking = (await GetTestBookings())
                    .Where(item => item.Id == 1)
                    .First();

            mockBookingsService.Setup(mock => mock.GetAsync(1))
                .ReturnsAsync(booking);
            // It.IsAny<System.Nullable<T>> not work if arg null
            mockBookingsService.Setup(mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockBookingsControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            mockBookingsControler.SetupGet(mock => mock.UserId).Returns((string)null);

            BookingUpdateDTO bookingUpdateDTO = new BookingUpdateDTO();

            await Assert.ThrowsAsync<BookingApp.Exceptions.OperationRestrictedException>(
                async () => await mockBookingsControler.Object.Terminate(1));

            mockBookingsService.Verify(
                mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        #endregion Delete

        #region Get lists

        [Fact]
        public async Task GetListProduceDTOsList()
        {
            Mock<IBookingsService> mockBookingsService = new Mock<IBookingsService>();
            Mock<BookingsController> mockBookingsControler = new Mock<BookingsController>(mockBookingsService.Object) { CallBase = true };

            mockBookingsService.Setup(mock => mock.ListBookings(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(await GetTestBookings());

            var result = await mockBookingsControler.Object.GetAll(null, null);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<BookingAdminDTO>>(okResult.Value);

            mockBookingsService.Verify(
                mock => mock.Terminate(It.IsAny<int>(), It.IsAny<string>()), Times.Never());

            Assert.Equal((await GetTestBookings()).Count(), model.Count());
            Assert.Equal((await GetTestBookings()).Where(p => p.Id == 1).FirstOrDefault().Note, (model.Where(p => p.Id == 1)).FirstOrDefault().Note);
        }

        #endregion Get lists

        #region Utility

        private async Task<IEnumerable<Booking>> GetTestBookings()
        {
            return await Task.Run(() =>
                new List<Booking>
                {
                    new Booking { Id = 1, Note = "1", ResourceId = 1, CreatedUserId = "1", UpdatedUserId = "1", CreatedTime = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, UpdatedTime = DateTime.Now},
                    new Booking { Id = 2, Note = "2", ResourceId = 1, CreatedUserId = "1", UpdatedUserId = "2", CreatedTime = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, UpdatedTime = DateTime.Now},
                    new Booking { Id = 3, Note = "3", ResourceId = 1, CreatedUserId = "1", UpdatedUserId = "3", CreatedTime = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, UpdatedTime = DateTime.Now},
                    new Booking { Id = 4, Note = "4", ResourceId = 1, CreatedUserId = "1", UpdatedUserId = "4", CreatedTime = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, UpdatedTime = DateTime.Now},
                }
            );
        }

        #endregion Utility
    }
}