using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService : IResourcesService
    {
        readonly IResourcesRepository resourcesRepo;

        public ResourcesService(IResourcesRepository resourcesRepo, ApplicationDbContext dbContext)
        {
            this.resourcesRepo = resourcesRepo;
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
        
    }

    public interface IResourcesService
    {
        Task Create(Resource resource);
        Task<IEnumerable<Resource>> GetList();
        Task<Resource> Get(int resourceId);
        Task Update(Resource resource);
        Task Delete(int resourceId);

        Task<IEnumerable<int>> ListKeys();

        Task<bool> IsActive(int resourceId);
        Task<IEnumerable<Resource>> ListActive();
        Task<IEnumerable<int>> ListActiveKeys();

        /// <summary>
        /// Lists all resources associated with the specified user. 
        /// </summary>
        Task<IEnumerable<Resource>> ListByAssociatedUser(string userId);

        /// <summary>
        /// Lists all resources adhering to the specified rule. 
        /// </summary>
        Task<IEnumerable<Resource>> ListByRuleKey(int ruleId);

        /// <summary>
        /// Lists all resources having specified parent tree group. 
        /// </summary>
        Task<IEnumerable<Resource>> ListByFolderKey(int folderId);
    }
}