using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Rule> Get(int id)
        {
            return await _rulesRepository.GetAsync(id);
        }

        public async Task Create(Rule rule)
        {
            CheckIfCorrect(rule);
            rule.CreatedTime = rule.UpdatedTime = DateTime.Now;
            await _rulesRepository.CreateAsync(rule);
        }

        public async Task Delete(int id)
        {
            await _rulesRepository.DeleteAsync(id);
        }

        public async Task Update(int id, Rule rule)
        {
            CheckIfCorrect(rule);
            rule.Id = id;
            rule.UpdatedTime = DateTime.Now;
            await _rulesRepository.UpdateAsync(rule);
        }

        public async Task<IEnumerable<Rule>> GetActiveList() => await _rulesRepository.ListActiveAsync();

        delegate bool Verify(Rule rule);

        public void CheckIfCorrect(Rule rule)
        {

            bool checkLogic1(Rule a)
            {
                if (a.MinTime >= a.MaxTime)        //min time can't > max time
                    return false;
                return true;
            }

            bool checkLogic2(Rule a)
            {
                var mV = 14400;                  //time of resource usage + service time + step time can't be greater than 14400
                if (a.MinTime > (mV - a.StepTime - a.ServiceTime) || a.MaxTime > (mV - a.StepTime - a.ServiceTime))  
                    return false;
                return true;
            }

            Dictionary<string, Verify> Verifications = new Dictionary<string, Verify>
            {
                { "Min time can't equal or be greater than max time", new Verify(checkLogic1) },
                { "Sum of resource usage time, step time and service time can't be greater than 14400", new Verify(checkLogic2) }
            };

            foreach (var key in Verifications)
            {
                if (key.Value(rule) == false)
                    throw new FieldValueTimeInvalidException(key.Key);
            }
        }

    }
}
