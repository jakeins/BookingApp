using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class RuleRepository : IRepositoryAsync<Rule, int>
    {
        ApplicationDbContext _db;

        public RuleRepository(ApplicationDbContext db)
        {
            _db = db;
        }                                                               //Change exception types

        public async Task CreateAsync(Rule rule)
        {
            try
            {
                _db.Rules.Add(rule);
                await SaveAsync();
            }
            catch (OperationFailedException e)
            {
                throw e;
            }
        }

        public async Task DeleteAsync(int id)
        {
            Rule rule = _db.Rules.FirstOrDefault(p => p.Id == id);

            if (rule != null)
            {
                try
                {
                    _db.Rules.Remove(rule);
                    await SaveAsync();
                }
                catch (SqlException e)
                {
                    Helpers.SqlExceptionTranslator.ReThrow(e, "on deleting rule");
                }
            }
            else throw new EntryNotFoundException();
        }

        public async Task UpdateAsync(Rule rule)
        {
            bool exists = await _db.Rules.AnyAsync(p => p.Id == rule.Id);

            if (exists == true)
            {
                try
                {
                    _db.Rules.Update(rule);
                    await SaveAsync();

                }
                catch (SqlException e)
                {
                    Helpers.SqlExceptionTranslator.ReThrow(e, "on updating rule");
                }
            }
            else throw new EntryNotFoundException();
        }

        public async Task<IEnumerable<Rule>> GetListAsync()
        {
            return await _db.Rules.ToListAsync();
        }

        public async Task<Rule> GetAsync(int id)
        {

            Rule rule = _db.Rules.FirstOrDefault(p => p.Id == id);
            //await SaveAsync();
            if (rule != null)
                return rule;
            else
                throw new EntryNotFoundException();
        }


        public async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbuException)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException);
            }
        }


    }
}
