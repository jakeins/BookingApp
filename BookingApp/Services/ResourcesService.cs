using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService
    {
        readonly IResourcesRepository resourcesRepo;

        public ResourcesService(ResourcesRepository resourcesRepo, ApplicationDbContext dbContext)
        {
            this.resourcesRepo = resourcesRepo;
        }

        public async Task<IEnumerable<Resource>> GetList() => await resourcesRepo.GetListAsync();

        public async Task<IEnumerable<Resource>> ListActive() => await resourcesRepo.ListActiveAsync();

        public async Task<Resource> Get(int resourceId) => await resourcesRepo.GetAsync(resourceId);

        public async Task Create(Resource resource) => await resourcesRepo.CreateAsync(resource);

        public async Task Update(Resource resource) => await resourcesRepo.UpdateAsync(resource);

        public async Task Delete(int resourceId) => await resourcesRepo.DeleteAsync(resourceId);

        public async Task<bool> IsActive(int resourceId) => await resourcesRepo.IsActiveAsync(resourceId);

        public async Task<IEnumerable<int>> ListKeys(bool includeIncativeResources) => await resourcesRepo.ListKeysAsync();

        public async Task<IEnumerable<int>> ListActiveKeys(bool includeIncativeResources) => await resourcesRepo.ListActiveKeysAsync();

        /// <summary>
        /// Lists all resources adhering to the specified rule. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByRuleKey(int ruleId) => await resourcesRepo.ListByRuleKeyAsync(ruleId);

        /// <summary>
        /// Lists all resources associated with the specified user. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByAssociatedUser(string userId) => await resourcesRepo.ListByAssociatedUser(userId);
    }
}