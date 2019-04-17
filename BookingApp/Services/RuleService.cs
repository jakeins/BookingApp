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


        public void CheckIfCorrect(Rule rule)
        {
            var mV = 14400;                  // max allowed value
            var min = rule.MinTime;
            var max = rule.MaxTime;
            var step = rule.StepTime;
            var service = rule.ServiceTime;
            string errorMessage = null;

            while (errorMessage == null) {

                if (min >= max)                                                          //min time can't > max time
                {
                    errorMessage = "Min time can't equal or be greater than max time";
                    break;
                }

                if (min > (mV - step - service) || max > (mV - step - service))          //time of resource usage + service time + step time can't be greater than 14400
                {
                    errorMessage = "Sum of resource usage time, step time and service time can't be greater than 14400";
                    break;
                }


                break;
           }

            if (errorMessage != null)                         
            {
                throw new FieldValueTimeInvalidException(errorMessage);
            }

        }
    }
        
}
