using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository with basic functionality.
    /// </summary>
    /// <typeparam name="TModel">Type of the repository item object.</typeparam>
    /// <typeparam name="TKey">Type of the primary key of the item object.</typeparam>
    public interface IBasicRepositoryAsync<TModel, TKey>
        where TModel : class
    {
        /// <summary>
        /// List all entries.
        /// </summary>
        Task<IEnumerable<TModel>> GetListAsync();

        /// <summary>
        /// Gets specified entry.
        /// </summary>
        Task<TModel> GetAsync(TKey key);

        /// <summary>
        /// Creates provided enitity in the storage.
        /// </summary>
        Task CreateAsync(TModel model);

        /// <summary>
        /// Updates storage with the provided entity.
        /// </summary>
        Task UpdateAsync(TModel model);

        /// <summary>
        /// Deletes specified entity.
        /// </summary>
        Task DeleteAsync(TKey key);

        /// <summary>
        /// Save changes to storage.
        /// </summary>
        Task SaveAsync();
    }
}
