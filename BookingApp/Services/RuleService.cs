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
            bool isCorrect = true;
            var mC = 14400;                  // max allowed value
            var min = rule.MinTime;
            var max = rule.MaxTime;
            var step = rule.StepTime;
            var service = rule.ServiceTime;
            var reuse = rule.ReuseTimeout;
            var preorder = rule.PreOrderTimeLimit;
            string errorMessage = null;

            while (isCorrect == true) {

                if (min <= 0 || max <= 0 || step < 0 || service < 0 || reuse < 0 || preorder < 0)  //check for correct values
                {                               
                    isCorrect = false;
                    errorMessage = "Time value can't be lower than 0, min and max time can't equal zero";
                    break;
                }

                if (min >= mC || max >= mC || step >= mC || service >= mC || reuse >= mC || preorder >= mC)
                {
                    errorMessage = "Time value can't equal or be greater than 14400";
                    isCorrect = false;
                    break;
                }

                if(min >= max)
                {
                    errorMessage = "Min time can't equal or be greater than max time";
                    isCorrect = false;
                    break;
                }

                if( min > (mC - max - step - service))
                {
                    errorMessage = "Min time can't be greater than 14400 - (max time + service time + step time)";
                    isCorrect = false;
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
