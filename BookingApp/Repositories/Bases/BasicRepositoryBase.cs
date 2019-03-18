using BookingApp.Data;
using BookingApp.Data.Models.Interfaces;
using BookingApp.Exceptions;
using BookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Bases
{
    /// <summary>
    /// Base class for basic repository that uses EF.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the primary id.</typeparam>
    public abstract class BasicRepositoryBase<TEntity, TEntityKey>
        : IBasicRepositoryAsync<TEntity, TEntityKey>
        where TEntity : class, IIdentifiable<TEntityKey>
        where TEntityKey : IEquatable<TEntityKey>
    {
        protected ApplicationDbContext dbContext;

        /// <summary>
        /// Name of the current entity, default is the model class name.
        /// </summary>
        protected string EntityName = typeof(TEntity).Name;

        /// <summary>
        /// Shorthand DbSet for Entities.
        /// </summary>
        protected DbSet<TEntity> Entities => dbContext.Set<TEntity>();

        /// <summary>
        /// Not found exception factory
        /// </summary>
        protected CurrentEntryNotFoundException NewNotFoundException => new CurrentEntryNotFoundException($"Specified {EntityName} not found");

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicRepositoryBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region Basic Repository Implementation

        public virtual async Task<IEnumerable<TEntity>> GetListAsync() => await Entities.ToListAsync();
        
        public virtual async Task<TEntity> GetAsync(TEntityKey id)
        {
            if (await Entities.SingleOrDefaultAsync(e => e.Id.Equals(id)) is TEntity entity)
                return entity;
            else
                throw NewNotFoundException;
        }
        
        public virtual async Task CreateAsync(TEntity entity)
        {
            Entities.Add(entity);
            await SaveAsync();
        }
        
        public virtual async Task UpdateAsync(TEntity entity)
        {
            var exists = (await GetAsync(entity.Id)) is TEntity;
            Entities.Update(entity);
            await SaveAsync();
        }            
        
        public virtual async Task DeleteAsync(TEntityKey id)
        {
            if (await GetAsync(id) is TEntity entity)
            {
                Entities.Remove(entity);

                await SaveAsync();
            }
            else
                throw NewNotFoundException;
        }

        public virtual async Task SaveAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbuException)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException, $"{EntityName} modification");
            }
        }
        #endregion
    }
}