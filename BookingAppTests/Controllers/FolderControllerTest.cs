using BookingApp.Controllers;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.DTOs.Folder;
using BookingApp.Exceptions;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BookingAppTests.Controllers
{
    public class FolderControllerTest
    {

        private readonly Mock<IFolderService> mockFolderService;

        public FolderControllerTest()
        {
            mockFolderService = new Mock<IFolderService>();
        }

        #region FolderController.Index
        [Fact]
        public async Task GetFoldersIfUserAuthAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.GetFolders()).ReturnsAsync(await GetTestFolders());
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.IsAdmin).Returns(true);
            // Act
            var result = await mockControler.Object.Index();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<FolderBaseDto>>(okResult.Value);

            // Assert
            Assert.Equal((await GetTestFolders()).Count(), model.Count());
            Assert.Equal((await GetTestFolders()).FirstOrDefault(p => p.Id == 1).Title, model.FirstOrDefault(p => p.Id == 1).Title);
        }

        [Fact]
        public async Task GetFoldersIfNotUserAuthAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.GetFoldersActive()).ReturnsAsync(await GetTestFolders());
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.IsAdmin).Returns(false);
            // Act
            var result = await mockControler.Object.Index();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<FolderBaseDto>>(okResult.Value);

            // Assert
            Assert.Equal((await GetTestFolders()).Count(), model.Count());
            Assert.Equal((await GetTestFolders()).Where(p => p.Id == 1).FirstOrDefault().Title, (model.Where(p => p.Id == 1)).FirstOrDefault().Title);
        }
        #endregion FolderController.Index

        #region FolderController.Detail
        [Fact]
        public async Task GetFolderByIdAsync()
        {
            // Arrange
            int id = 2;
            mockFolderService.Setup(service => service.GetDetail(id)).ReturnsAsync((await GetTestFolders()).FirstOrDefault(p => p.Id == 2));
            FolderController controller = new FolderController(mockFolderService.Object);

            // Act
            var result = await controller.Detail(id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<FolderBaseDto>(okResult.Value);

            // Assert
            Assert.Equal((await GetTestFolders()).FirstOrDefault(f => f.Id == id).Title, model.Title);
        }

        [Fact]
        public async void GetFolderByIdFailedAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.GetDetail(It.IsAny<int>())).Throws(new CurrentEntryNotFoundException("Specified Folder not found"));
            FolderController controller = new FolderController(mockFolderService.Object);

            var ex = await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => controller.Detail(It.IsAny<int>()));
            Assert.Equal("Specified Folder not found", ex.Message);
        }
        #endregion FolderController.Detail

        #region FolderController.Create
        [Fact]
        public async void CreateOkFolderAsync()
        {
            // Arrange
            FolderMinimalDto FolderDto = GetDtoCorrect();
            Mock<Folder> mockFolder = new Mock<Folder>();
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());
            mockControler.SetupGet(mock => mock.BaseApiUrl).Returns(It.IsAny<string>());

            // Act
            var result = await mockControler.Object.Create(FolderDto);

            // Assert
            Assert.IsType<CreatedResult>(result);
            mockFolderService.Verify(
                mock => mock.Create(It.IsAny<string>(), mockFolder.Object), Times.Never());
        }

        [Fact]
        public async void IsBadRequestObjectResultCreateFolder()
        {
            // Arrange
            Mock<FolderMinimalDto> mockDto = new Mock<FolderMinimalDto>();
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());
            mockControler.SetupGet(mock => mock.BaseApiUrl).Returns(It.IsAny<string>());

            // Act
            mockControler.Object.ModelState.AddModelError("Title", "Title should be no more 64 characters");
            var result = await mockControler.Object.Create(mockDto.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion FolderController.Create

        #region FolderController.Update
        [Fact]
        public async void UpdateOkFolderAsync()
        {
            // Arrange
            FolderMinimalDto FolderDto = GetDtoCorrect();
            Mock<Folder> mockFolder = new Mock<Folder>();
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            // Act
            var result = await mockControler.Object.Update(It.IsAny<int>(), FolderDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockFolderService.Verify(
                mock => mock.Update(It.IsAny<int>(), It.IsAny<string>(), mockFolder.Object), Times.Never());
        }

        [Fact]
        public async void IsBadRequestObjectResultUpdateFolder()
        {
            // Arrange
            Mock<FolderMinimalDto> mockDto = new Mock<FolderMinimalDto>();

            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };
            mockControler.SetupGet(mock => mock.UserId).Returns(It.IsAny<string>());

            // Act
            mockControler.Object.ModelState.AddModelError("Title", "Title should be no more 64 characters");
            var result = await mockControler.Object.Update(It.IsAny<int>(), mockDto.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion FolderController.Update

        #region FolderController.Delete
        [Fact]
        public async void DeleteOkFolderAsync()
        {
            // Arrange
            Mock<FolderController> mockControler = new Mock<FolderController>(mockFolderService.Object) { CallBase = true };

            // Act
            var result = await mockControler.Object.Delete(It.IsAny<int>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockFolderService.Verify(
                mock => mock.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async void GetFolderByIdFailedDeleteFolderAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.Delete(It.IsAny<int>())).Throws(new CurrentEntryNotFoundException("Specified Folder not found"));
            FolderController controller = new FolderController(mockFolderService.Object);

            // Assert
            var ex = await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => controller.Delete(It.IsAny<int>()));
            Assert.Equal("Specified Folder not found", ex.Message);
        }
        #endregion FolderController.Delete



        #region Utility
        private async Task<IEnumerable<Folder>> GetTestFolders()
        {
            return await Task.Run(() =>
                new List<Folder>
                {
                    new Folder { Id=1, Title="Folders_1", ParentFolderId = null},
                    new Folder { Id=2, Title="Folders_2", ParentFolderId = 1},
                    new Folder { Id=3, Title="Folders_3", ParentFolderId = 1},
                    new Folder { Id=4, Title="Folders_4", ParentFolderId = null}
                }
            );
        }

        private FolderMinimalDto GetDtoCorrect()
        {
            return new FolderMinimalDto
            {
                Title = "Folder 1",
                ParentFolderId = 1,
                DefaultRuleId = 1,
                IsActive = true
            };
        }
        #endregion Utility

    }
}
