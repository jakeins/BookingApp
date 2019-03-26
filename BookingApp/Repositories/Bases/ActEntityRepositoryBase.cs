using BookingApp.Data;
using BookingApp.Data.Models.Interfaces;
using BookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Bases
{
    /// <summary>
    /// Base class for repository that uses EF db context with Activable Entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the primary id.</typeparam>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the primary id of the related user.</typeparam>
    public abstract class ActEntityRepositoryBase<TEntity, TEntityKey, TUserModel, TUserKey>
        : TrackEntityRepositoryBase<TEntity, TEntityKey, TUserModel, TUserKey>,
        IActEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        where TEntity : class, IIdentifiable<TEntityKey>, ITrackable<TUserModel, TUserKey>, IActivable
        where TEntityKey : IEquatable<TEntityKey>
        where TUserModel : class
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// IQueryable shorthand for only active Entities.
        /// </summary>
        protected IQueryable<TEntity> ActiveEntities => Entities.Where(e => e.IsActive != false);

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActEntityRepositoryBase(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> IsActiveAsync(TEntityKey id)
        {
            var result = await Entities.Where(e => e.Id.Equals(id)).Select(e => new { e.IsActive }).SingleOrDefaultAsync();

            if (result != null)
                return result.IsActive != false;
            else
                throw NewNotFoundException;
        }
        
        public async Task<IEnumerable<TEntity>> ListActiveAsync() => await ActiveEntities.ToListAsync();
        public async Task<IEnumerable<TEntityKey>> ListActiveKeysAsync() => await ActiveEntities.Select(e => e.Id).ToListAsync();        
        public async Task<int> CountActiveAsync() => await ActiveEntities.CountAsync();
    }
}