using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
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

        #region MethodsForStatisticsService

        public async Task<IEnumerable<Resource>> ListIncludingBookingsAndRules() => await Resources.Include(r => r.Bookings).Include(r => r.Rule).ToListAsync();

        public async Task<Resource> GetIncludingBookingsAndRules(int resourceID) => await Resources.Include(r => r.Bookings).Include(r => r.Rule).SingleOrDefaultAsync(r => r.Id == resourceID);

        #endregion
    }
}