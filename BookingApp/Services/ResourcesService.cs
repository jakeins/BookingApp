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

        #region CRUD operations
        /// <summary>
        /// List resources, all or just the active ones.
        /// </summary>
        public async Task<IEnumerable<Resource>> List(bool includeInactiveResources)
        {
            if(includeInactiveResources)
                return await resourcesRepo.GetListAsync();
            else
                return await resourcesRepo.ListActiveAsync();
        }

        public async Task<Resource> Single(int resourceId) => await resourcesRepo.GetAsync(resourceId);

        public async Task Create(Resource resource) => await resourcesRepo.CreateAsync(resource);

        public async Task Update(Resource resource) => await resourcesRepo.UpdateAsync(resource);

        public async Task Delete(int resourceId) => await resourcesRepo.DeleteAsync(resourceId);
        #endregion

        #region Extended operations
        /// <summary>
        /// Check whether specified resource is active.
        /// </summary>
        public async Task<bool> IsActive(int resourceId) => await resourcesRepo.IsActiveAsync(resourceId);

        /// <summary>
        /// List identifiers of resources, all or just the active ones.
        /// </summary>
        public async Task<IEnumerable<int>> ListIDs(bool includeIncativeResources)
        {
            return includeIncativeResources ? await resourcesRepo.ListKeysAsync() : await resourcesRepo.ListActiveKeysAsync();
        }

        /// <summary>
        /// Lists all resources adhering to the specified rule. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByRule(int ruleId) => await resourcesRepo.ListByRuleAsync(ruleId);

        /// <summary>
        /// Lists all resources associated with the specified user. 
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByAssociatedUser(string userId) => await resourcesRepo.ListByAssociatedUser(userId);
        #endregion
    }
}