using BookingApp.Controllers;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingAppTests
{
    public class FolderControllerTest
    {
        private readonly Mock<IFolderService> mockFolderService;

        public FolderControllerTest()
        {
            mockFolderService = new Mock<IFolderService>();
            //var mockBaseController = new Mock<EntityControllerBase>();
            //mockBaseController.Setup(bc => bc.IsAdmin).Returns(true);
        }

        #region FolderController.Index
        [Fact]
        public async Task GetFoldersIfUserAuthAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.GetFolders()).ReturnsAsync(await GetTestFolders());
            FolderController controller = new FolderController(mockFolderService.Object);
            controller.isUserT = true;

            // Act
            var result = await controller.Index();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<FolderBaseDto>>(okResult.Value);

            // Assert
            Assert.Equal(GetTestFolders().GetAwaiter().GetResult().Count(), model.Count());
            Assert.Equal(GetTestFolders().GetAwaiter().GetResult().Where(p => p.Id == 1).FirstOrDefault().Title, (model.Where(p => p.Id == 1)).FirstOrDefault().Title);
        }

        [Fact]
        public async Task GetFoldersIfNotUserAuthAsync()
        {
            // Arrange
            mockFolderService.Setup(service => service.GetFoldersActive()).ReturnsAsync(await GetTestFolders());
            FolderController controller = new FolderController(mockFolderService.Object);
            controller.isUserT = false;

            // Act
            var result = await controller.Index();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<FolderBaseDto>>(okResult.Value);

            // Assert
            Assert.Equal(GetTestFolders().GetAwaiter().GetResult().Count(), model.Count());
            Assert.Equal(GetTestFolders().GetAwaiter().GetResult().Where(p => p.Id == 1).FirstOrDefault().Title, (model.Where(p => p.Id == 1)).FirstOrDefault().Title);
        }
        #endregion FolderController.Index

        #region FolderController.Detail
        [Fact]
        public async Task GetFolderByIdAsync()
        {
            // Arrange
            int id = 2;
            mockFolderService.Setup(service => service.GetDetail(id)).ReturnsAsync(GetTestFolders().GetAwaiter().GetResult().Where(p => p.Id == 2).FirstOrDefault());
            FolderController controller = new FolderController(mockFolderService.Object);

            // Act
            var result = await controller.Detail(id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<FolderBaseDto>(okResult.Value);

            // Assert
            Assert.Equal(GetTestFolders().GetAwaiter().GetResult().Where(f => f.Id == id).FirstOrDefault().Title, model.Title);
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
        public async void CreateFolderAsync()
        {
            // Arrange
            FolderMinimalDto FolderDto = new FolderMinimalDto
            {
                Title = "Folder 1",
                ParentFolderId = 1,
                DefaultRuleId = 1,
                IsActive = true
            };
            
            FolderController controller = new FolderController(mockFolderService.Object);

            // Act
            var result = await controller.Create(FolderDto);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }
        #endregion FolderController.Create



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
        #endregion Utility

    }
}
