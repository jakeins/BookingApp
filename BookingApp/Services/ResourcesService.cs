using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService : IResourcesService
    {
        readonly IResourcesRepository resourcesRepo;
        readonly IBookingsRepository bookingsRepo;

        public ResourcesService(IResourcesRepository resourcesRepo, IBookingsRepository bookingsRepo)
        {
            this.resourcesRepo = resourcesRepo;
            this.bookingsRepo = bookingsRepo;
        }

        public async Task Create(Resource resource) => await resourcesRepo.CreateAsync(resource);
        public async Task<IEnumerable<Resource>> GetList() => await resourcesRepo.GetListAsync();
        public async Task<Resource> Get(int resourceId) => await resourcesRepo.GetAsync(resourceId);
        public async Task Update(Resource resource) => await resourcesRepo.UpdateAsync(resource);
        public async Task Delete(int resourceId) => await resourcesRepo.DeleteAsync(resourceId);

        public async Task<IEnumerable<int>> ListKeys() => await resourcesRepo.ListKeysAsync();
               
        public async Task<bool> IsActive(int resourceId) => await resourcesRepo.IsActiveAsync(resourceId);
        public async Task<IEnumerable<Resource>> ListActive() => await resourcesRepo.ListActiveAsync();
        public async Task<IEnumerable<int>> ListActiveKeys() => await resourcesRepo.ListActiveKeysAsync();

        public async Task<IEnumerable<Resource>> ListByAssociatedUser(string userId) => await resourcesRepo.ListByAssociatedUser(userId);
        public async Task<IEnumerable<Resource>> ListByRuleKey(int ruleId) => await resourcesRepo.ListByRuleKeyAsync(ruleId);
        public async Task<IEnumerable<Resource>> ListByFolderKey(int folderId) => await resourcesRepo.ListByFolderKeyAsync(folderId);

        #region Occupancy
        public virtual async Task<double?> OccupancyByResource(int resourceId) => await bookingsRepo.OccupancyByResourceAsync(resourceId);

        public async Task<Dictionary<int, double?>> GetOccupancies()
        {
            var idsList = await ListKeys();
            return await GetOccupanciesByIDs(idsList);
        }

        public async Task<Dictionary<int, double?>> GetActiveOccupancies()
        {
            var idsList = await ListActiveKeys();
            return await GetOccupanciesByIDs(idsList);
        }

        public async Task<Dictionary<int, double?>> GetOccupanciesByIDs(IEnumerable<int> idsList)
        {
            var map = new Dictionary<int, double?>();

            foreach (int resourceId in idsList)
            {
                map.Add(resourceId, null);

                try
                {
                    map[resourceId] = await OccupancyByResource(resourceId);
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FieldValueAbsurdException)
                {
                    //silently swallowing disjoint values
                }
            }
            return map;
        }
        #endregion
    }
}