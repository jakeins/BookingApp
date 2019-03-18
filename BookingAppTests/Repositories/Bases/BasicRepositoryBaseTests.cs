using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories;
using BookingApp.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TestingUtilities;
using Xunit;

namespace BookingAppTests.Repositories
{
    /// <summary>
    /// Testing class using Resource entity as a real entity
    /// </summary>
    public class BasicRepositoryBaseTests
    {
        #region GetListAsync() tests
        [Fact]
        public async void GetListAsync_ReturnsAllResources()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            
            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                var result = await repo.GetListAsync();

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<Resource>>(result);
            }
        }
        #endregion

        #region GetAsync() tests
        [Fact]
        public async void GetAsync_ReturnsResource()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldSet = ResourceUtils.TestSet;

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                var result = await repo.GetAsync(context.Resources.First().Id);

                //Assert
                Assert.IsAssignableFrom<Resource>(result);
            }
        }

        [Fact]
        public async void GetAsync_ThrowsNotFound_OnNonExistent()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldModel = ResourceUtils.TestSet.First();
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(oldModel);
                context.SaveChanges();
            }
            var newModel = ResourceUtils.TestSet.Last();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);

                //Assert-Act
                await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => repo.GetAsync(ResourceUtils.NonExistentId));
            }
        }
        #endregion

        #region CreateAsync() tests
        [Fact]
        public async void CreateAsync_ChangesQtyAndSavesModelRight()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            var expectedCount = 1;
            var addedModel = ResourceUtils.TestSet.First();

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                await repo.CreateAsync(addedModel);
            }

            //Assert
            using (var context = new ApplicationDbContext(options))
            {
                Assert.Equal(addedModel.Title, context.Resources.First().Title);
                Assert.Equal(expectedCount, context.Resources.Count());
            }
        }
        #endregion

        #region UpdateAsync() tests
        [Fact]
        public async void UpdateAsync_ChangesFields()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldModel = ResourceUtils.TestSet.First();
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(oldModel);
                context.SaveChanges();
            }
            var newModel = ResourceUtils.TestSet.Last();

            //Act
            using (var context = new ApplicationDbContext(contextOptions))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                newModel.Id = oldModel.Id;
                await repo.UpdateAsync(newModel);
            }

            //Assert
            using (var context = new ApplicationDbContext(contextOptions))
            {
                Assert.Equal(newModel.Title, context.Resources.First().Title);
                Assert.NotEqual(oldModel.Title, context.Resources.First().Title);
            }
        }

        [Fact]
        public async void UpdateAsync_ThrowsNotFound_OnNonExistent()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldModel = ResourceUtils.TestSet.First();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(oldModel);
                context.SaveChanges();
            }
            var newModel = ResourceUtils.TestSet.Last();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                newModel.Id = ResourceUtils.NonExistentId;

                //Assert-Act
                await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => repo.UpdateAsync(newModel));
                Assert.Equal(oldModel.Title, context.Resources.First().Title);
            }
        }
        #endregion

        #region DeleteAsync() tests
        [Fact]
        public async void DeleteAsync_ChangesQuantity()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldModel = ResourceUtils.TestSet.First();
            int oldQuantity;
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(oldModel);
                context.SaveChanges();
                oldQuantity = context.Resources.Count();
            }

            //Act
            using (var context = new ApplicationDbContext(contextOptions))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                await repo.DeleteAsync(oldModel.Id);
            }

            //Assert
            using (var context = new ApplicationDbContext(contextOptions))
            {
                Assert.Equal(1, oldQuantity);
                Assert.Empty(context.Resources);
            }
        }

        [Fact]
        public async void DeleteAsync_ThrowsNotFound_OnNonExistent()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();
            var oldModel = ResourceUtils.TestSet.First();
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(oldModel);
                context.SaveChanges();
            }
            var newModel = ResourceUtils.TestSet.Last();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);

                //Assert-Act
                await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => repo.DeleteAsync(ResourceUtils.NonExistentId));
            }
        }
        #endregion

        #region SaveAsync() tests
        [Fact]
        public async void SaveAsync_ChangesQuantity_OnAdd()
        {
            //Arrange
            var contextOptions = InMemoryUtils.ProduceFreshDbContextOptions();

            //Act
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());

                IBasicRepositoryAsync<Resource, int> repo = new ResourcesRepository(context);
                await repo.SaveAsync();
            }

            //Assert
            using (var context = new ApplicationDbContext(contextOptions))
            {
                Assert.NotEmpty(context.Resources);
            }
        }
        #endregion
    }
}