using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public interface IRepository<TModel, TKey>
        where TModel : class
    {
        IEnumerable<TModel> GetList();
        TModel Get(TKey id);
        void Create(TModel model);
        void Update(TModel model);
        void Delete(TKey id);
        void Save();
    }

    public interface IRepositoryAsync<TModel, TKey>
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
