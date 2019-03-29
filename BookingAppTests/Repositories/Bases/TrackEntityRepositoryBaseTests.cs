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
    public class TrackEntityRepositoryBaseTests
    {
        #region UpdateSelectiveAsync() tests

        class ResourceTestUpdateSubset
        {
            public string Title { get; set; }
        }

        [Fact]
        public async void UpdateSelectiveAsync_ChangesFieldSubsetOnly()
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
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                newModel.Id = oldModel.Id;
                await repo.UpdateSelectiveAsync<ResourceTestUpdateSubset>(newModel);
            }

            //Assert
            using (var context = new ApplicationDbContext(contextOptions))
            {
                Assert.NotEqual(oldModel.Title, context.Resources.First().Title);
                Assert.Equal(newModel.Title, context.Resources.First().Title);
                Assert.NotEqual(newModel.Description, context.Resources.First().Description);
            }
        }

        [Fact]
        public async void UpdateSelectiveAsync_ThrowsNotFound_OnNonExistent()
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
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                newModel.Id = ResourceUtils.NonExistentId;

                //Assert-Act
                await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => repo.UpdateSelectiveAsync<ResourceTestUpdateSubset>(newModel));
                Assert.Equal(oldModel.Title, context.Resources.First().Title);
            }
        }
        #endregion

        #region ListKeysAsync() tests
        [Fact]
        public async void ListKeysAsync_ReturnsAllKeys()
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
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListKeysAsync();

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<int>>(result);
                Assert.Single(result);
            }
        }
        #endregion

        #region ExistsAsync() [2] tests
        [Theory]
        [InlineData(1, true)]
        [InlineData(999, false)]
        public async void ExistsAsync_ReturnsCorrectBool(int id, bool expected)
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.AddRange(ResourceUtils.TestSet);

                context.SaveChanges();
            }

            var model = ResourceUtils.TestSet.Any(r => r.Id == id) ? ResourceUtils.TestSet.Single(r => r.Id == id) : new Resource() { Id = 999 };

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var resultKeyed = await repo.ExistsAsync(id);
                var resultModeled = await repo.ExistsAsync(model);

                //Assert
                Assert.IsAssignableFrom<bool>(resultKeyed);
                Assert.Equal(expected, resultKeyed);
                Assert.IsAssignableFrom<bool>(resultModeled);
                Assert.Equal(expected, resultModeled);
            }
        }
        #endregion

        #region ListByAssociatedUser() tests
        [Fact]
        public async void ListByAssociatedUser_ReturnsCorrectResources()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            var user = ResourceUtils.VincentVanGogh;

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListByAssociatedUser(user);

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<Resource>>(result);
                Assert.Single(result);
            }
        }
        #endregion

        #region ListByCreator() tests
        [Fact]
        public async void ListByCreator_ReturnsCorrectResources()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            var creator = ResourceUtils.VincentVanGogh;

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListByCreator(creator);

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<Resource>>(result);
                Assert.Single(result);
            }
        }
        #endregion

        #region ListByUpdater() tests
        [Fact]
        public async void ListByUpdater_ReturnsCorrectResources()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();
            var updater = ResourceUtils.VincentVanGogh;

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First());
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                ITrackEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListByUpdater(updater);

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<Resource>>(result);
                Assert.Single(result);
            }
        }
        #endregion
    }
}