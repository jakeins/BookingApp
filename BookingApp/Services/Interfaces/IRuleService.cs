using BookingApp.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services.Interfaces
{
    public interface IRuleService
    {
        Task<IEnumerable<Rule>> GetList();
        Task<Rule> Get(int id);
        Task Create(Rule rule);
        Task Delete(int id);
        Task Update(Rule rule);
        Task<IEnumerable<Rule>> GetActiveList();
        Task<bool> GetActive(int id);
    }
}
