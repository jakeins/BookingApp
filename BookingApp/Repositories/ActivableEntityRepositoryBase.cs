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
    /// Base class for repository that uses EF db context with Activable Entities.
    /// </summary>
    public abstract class ActivableEntityRepositoryBase<TEntity, TEntityKey, TUserKey> 
        : EntityRepositoryBase<TEntity, TEntityKey, TUserKey>
        where TEntity : class, IActivableEntity<TEntityKey, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// IQueryable shorthand for only active Entities.
        /// </summary>
        protected IQueryable<TEntity> ActiveEntities => Entities.Where(e => e.IsActive == true);

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivableEntityRepositoryBase(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Checks whether sepcified entity is active.
        /// </summary>
        public async Task<bool> IsActiveAsync(TEntityKey id)
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
        public async Task<IEnumerable<TEntity>> ListActiveAsync() => await ActiveEntities.ToListAsync();

        /// <summary>
        /// Lists identifiers of all active entities.
        /// </summary>
        public async Task<IEnumerable<TEntityKey>> ListActiveIDsAsync() => await ActiveEntities.Select(e => e.Id).ToListAsync();
    }
}