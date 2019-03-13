using BookingApp.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services.Interfaces
{
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

        #region Occupancy Interface
        /// <summary>
        /// Get ocuppancy calculation based on booking of the specified resource.
        /// </summary>
        Task<double?> OccupancyByResource(int resourceId);

        /// <summary>
        /// Get ocuppancies calculations based on all bookings.
        /// </summary>
        Task<Dictionary<int, double?>> GetOccupancies();

        /// <summary>
        /// Get ocuppancies calculations based on active bookings.
        /// </summary>
        Task<Dictionary<int, double?>> GetActiveOccupancies();
        #endregion
    }
}