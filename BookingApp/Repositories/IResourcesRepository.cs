using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Data.Models;

namespace BookingApp.Repositories
{
    /// <summary>
    /// Specific Resources Interface repository for the Booking App.
    /// </summary>
    public interface IResourcesRepository
        : IActEntityRepository<Resource, int, ApplicationUser, string>
    {
        /// <summary>
        /// Lists all resources adhering to the specified rule.
        /// </summary>
        Task<IEnumerable<Resource>> ListByRuleKeyAsync(int ruleId);
    }
}