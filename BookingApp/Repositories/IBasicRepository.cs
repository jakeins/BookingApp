using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public interface IBasicRepositoryAsync<TModel, TKey>
        where TModel : class
    {
        Task<IEnumerable<TModel>> GetListAsync();
        Task<TModel> GetAsync(TKey id);
        Task CreateAsync(TModel model);
        Task UpdateAsync(TModel model);
        Task DeleteAsync(TKey id);
        Task SaveAsync();
    }
}
