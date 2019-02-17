using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService
    {
        readonly ResourcesRepository resourcesRepo;

        public ResourcesService(ResourcesRepository resourcesRepo, ApplicationDbContext dbContext)
        {
            this.resourcesRepo = resourcesRepo;
        }

        public async Task<IEnumerable<Resource>> List() => await resourcesRepo.GetListAsync();

        public async Task<IEnumerable<Resource>> ListActive() => await resourcesRepo.ListActiveAsync();

        public async Task<Resource> Single(int resourceId) => await resourcesRepo.GetAsync(resourceId);

        public async Task Create(Resource resource) => await resourcesRepo.CreateAsync(resource);

        public async Task Update(Resource resource) => await resourcesRepo.UpdateAsync(resource);

        public async Task Delete(int resourceId) => await resourcesRepo.DeleteAsync(resourceId);

        public async Task<bool> IsActive(int resourceId) => await resourcesRepo.IsActiveAsync(resourceId);

        public async Task<IEnumerable<int>> ListIDs(bool includeIncativeResources) => await resourcesRepo.ListIDsAsync();

        public async Task<IEnumerable<int>> ListActiveIDs(bool includeIncativeResources) => await resourcesRepo.ListActiveIDsAsync();

        /// <summary>
        /// Lists all resources adhering to the specified rule. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByRule(int ruleId) => await resourcesRepo.ListByRuleAsync(ruleId);

        /// <summary>
        /// Lists all resources associated with the specified user. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByAssociatedUser(string userId) => await resourcesRepo.ListByAssociatedUser(userId);
    }
}