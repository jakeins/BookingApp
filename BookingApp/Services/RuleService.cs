using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Data.Models;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services.Interfaces;

namespace BookingApp.Services
{

    public class RuleService : IRuleService
    {
        readonly IRuleRepository _rulesRepository;

        public RuleService(IRuleRepository rulesRepository)
        {
            _rulesRepository = rulesRepository;
        }

        public async Task<IEnumerable<Rule>> GetList()
        {
            return await _rulesRepository.GetListAsync();
        }

        public async Task<Rule> Get(int id) => await _rulesRepository.GetAsync(id);

        public async Task Create(Rule rule) => await _rulesRepository.CreateAsync(rule);

        public async Task Delete(int id) => await _rulesRepository.DeleteAsync(id);

        public async Task Update(Rule rule) => await _rulesRepository.UpdateAsync(rule);

        public async Task<IEnumerable<Rule>> GetActiveList() => await _rulesRepository.ListActiveAsync();

        public async Task<bool> GetActive(int id) => await _rulesRepository.IsActiveAsync(id);
    }

}
