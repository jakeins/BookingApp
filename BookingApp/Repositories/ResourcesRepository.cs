using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Lists all resources having specified parent tree group.
        /// </summary>
        Task<IEnumerable<Resource>> ListByFolderKeyAsync(int folderId);
    }

    public class ResourcesRepository
        : ActEntityRepositoryBase<Resource, int, ApplicationUser, string>,
        IResourcesRepository
    {
        DbSet<Resource> Resources => Entities;

        public ResourcesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task UpdateAsync(Resource resource) => await UpdateSelectiveAsync<ResourceUpdateSubsetDto>(resource);

        public async Task<IEnumerable<Resource>> ListByRuleKeyAsync(int ruleId) => await Resources.Where(r => r.RuleId == ruleId).ToListAsync();

        public async Task<IEnumerable<Resource>> ListByFolderKeyAsync(int folderId) => await Resources.Where(r => r.FolderId == folderId).ToListAsync();
    }
}