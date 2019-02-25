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

        [Fact]
        public async void GetTestAsync()
        {
            // Arrange
            IEnumerable<Folder> fakeList = await GetTestFolders();
            var mockService = new Mock<IFolderService>();
            mockService.Setup(service => service.GetFoldersActive()).Returns(Task.FromResult(fakeList));
            FolderController controller = new FolderController(mockService.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<FolderBaseDto>>(okResult.Value);

            Assert.Equal(fakeList.Count(), model.Count());
            Assert.Equal(fakeList.Where(p => p.Id == 1).FirstOrDefault().Title, (model.Where(p => p.Id == 1)).FirstOrDefault().Title);
        }

        [Fact]
        public async void DetailTestAsync()
        {
            int id = 2;

            // Arrange
            IEnumerable<Folder> fakeList = await GetTestFolders();
            var mockService = new Mock<IFolderService>();
            mockService.Setup(service => service.GetDetail(id)).Returns(Task.FromResult(fakeList.Where(f => f.Id == id).FirstOrDefault()));
            FolderController controller = new FolderController(mockService.Object);

            // Act
            var result = await controller.Detail(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<FolderBaseDto>(okResult.Value);

            Assert.Equal(fakeList.Where(f => f.Id == id).FirstOrDefault().Title, model.Title);
        }

        [Fact]
        public async void DetailFailedNotFoundTestAsync()
        {
            int id = 5;

            // Arrange
            IEnumerable<Folder> fakeList = await GetTestFolders();
            var mockService = new Mock<IFolderService>();
            mockService.Setup(service => service.GetDetail(id)).Returns(Task.FromResult(fakeList.Where(f => f.Id == id).FirstOrDefault()));
            FolderController controller = new FolderController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => controller.Detail(id));
        }






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

    }
}
