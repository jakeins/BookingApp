using BookingApp.Data;
using BookingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    /// <summary>
    /// Base class for repository that uses EF db context.
    /// </summary>
    public abstract class ActivableEntityRepositoryBase<EntityType, EntityIdType> : EntityRepositoryBase<EntityType, EntityIdType>
        where EntityType : class, IActivableEntity<EntityIdType>
        where EntityIdType : IEquatable<EntityIdType>
    {
        /// <summary>
        /// IQueryable shorthand for only active Entities.
        /// </summary>
        protected IQueryable<EntityType> ActiveEntities => Entities.Where(e => e.IsActive == true);

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivableEntityRepositoryBase(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Checks whether sepcified entity is active.
        /// </summary>
        public async Task<bool> IsActiveAsync(EntityIdType id)
        {
            var result = await Entities.Where(e => e.Id.Equals(id)).Select(e => new { e.IsActive }).SingleOrDefaultAsync();

            if (result != null)
                return result.IsActive == true;
            else
                throw NewNotFoundException;
        }

        /// <summary>
        /// Lists active entities.
        /// </summary>
        public async Task<IEnumerable<EntityType>> ListActiveAsync() => await ActiveEntities.ToListAsync();

        /// <summary>
        /// Lists identifiers of all active entities.
        /// </summary>
        public async Task<IEnumerable<EntityIdType>> ListActiveIDsAsync() => await ActiveEntities.Select(e => e.Id).ToListAsync();
    }
}