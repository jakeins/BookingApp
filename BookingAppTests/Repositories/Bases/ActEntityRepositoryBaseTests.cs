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
    public class ActEntityRepositoryBaseTests
    {
        #region IsActiveAsync() tests
        [Theory]
        [InlineData(1, true)]
        [InlineData(6, false)]
        public async void IsActiveAsync_ReturnsCorrectBool(int id, bool expected)
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.AddRange(ResourceUtils.TestSet);

                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IActEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.IsActiveAsync(id);

                //Assert
                Assert.IsAssignableFrom<bool>(result);
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public async void IsActiveAsync_ThrowsNotFound_OnNonExistent()
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
                IActEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);

                //Assert-Act
                await Assert.ThrowsAsync<CurrentEntryNotFoundException>(() => repo.IsActiveAsync(ResourceUtils.NonExistentId));
            }
        }
        #endregion

        #region ListActiveAsync() tests
        [Fact]
        public async void ListActiveAsync_ReturnsActiveResources()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.Add(ResourceUtils.TestSet.First(r=>r.IsActive == true));
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IActEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListActiveAsync();

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<Resource>>(result);
            }
        }
        #endregion

        #region ListActiveKeysAsync() tests
        [Fact]
        public async void ListKeysAsync_ReturnsActiveKeys()
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
                IActEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.ListActiveKeysAsync();

                //Assert
                Assert.NotEmpty(result);
                Assert.IsAssignableFrom<IEnumerable<int>>(result);
                Assert.Single(result);
            }
        }
        #endregion


        #region CountActiveAsync() tests
        [Fact]
        public async void CountAsync_ReturnsCorrectCount()
        {
            //Arrange
            var options = InMemoryUtils.ProduceFreshDbContextOptions();

            using (var context = new ApplicationDbContext(options))
            {
                context.Resources.AddRange(ResourceUtils.TestSet);
                context.SaveChanges();
            }

            //Act
            using (var context = new ApplicationDbContext(options))
            {
                IActEntityRepository<Resource, int, ApplicationUser, string> repo = new ResourcesRepository(context);
                var result = await repo.CountActiveAsync();

                //Assert
                Assert.IsAssignableFrom<int>(result);
                Assert.Equal(ResourceUtils.TestSet.Count(r => r.IsActive == true), result);
            }
        }
        #endregion
    }
}