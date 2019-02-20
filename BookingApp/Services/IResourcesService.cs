using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Data.Models;

namespace BookingApp.Services
{
    public interface IResourcesService
    {
        Task Create(Resource resource);
        Task<IEnumerable<Resource>> GetList();
        Task<Resource> Get(int resourceId);
        Task Update(Resource resource);
        Task Delete(int resourceId);

        Task<IEnumerable<int>> ListKeys(bool includeIncativeResources);

        Task<bool> IsActive(int resourceId);
        Task<IEnumerable<Resource>> ListActive();
        Task<IEnumerable<int>> ListActiveKeys(bool includeIncativeResources);

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
        Task<IEnumerable<Resource>> ListByTreeGroupKey(int treeGroupId);

        
        
    }
}