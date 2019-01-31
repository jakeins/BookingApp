using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public interface IRepository<TModel>
        where TModel : class
    {
        IEnumerable<TModel> GetList();
        TModel Get<T>(T id);
        void Create(TModel model);
        void Update(TModel model);
        void Delete<T>(T id);
        void Save();
    }
}
