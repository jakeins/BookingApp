using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class ResourcesRepository : EntityRepositoryBase<Resource, int>, IRepositoryAsync<Resource, int>
    {
        public ResourcesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #region Standard repository operations
        public override async Task UpdateAsync(Resource resource)
        {
            await UpdateSelectiveAsync<ResourceUpdatePropertiesDto>(resource);
        }
        #endregion

        #region Extensions
        /// <summary>
        /// Lists all resources adhering to the specified rule.
        /// </summary>
        public async Task<IEnumerable<Resource>> ListByRuleAsync(int ruleId) => await Entities.Where(r => r.RuleId == ruleId).ToListAsync();
        #endregion
    }
}