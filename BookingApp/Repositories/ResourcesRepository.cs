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
        public ResourcesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task UpdateAsync(Resource resource) => await UpdateSelectiveAsync<ResourceUpdateSubsetDto>(resource);

        public async Task<IEnumerable<Resource>> ListByRuleKeyAsync(int ruleId) => await Entities.Where(r => r.RuleId == ruleId).ToListAsync();
    }
}