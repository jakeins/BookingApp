using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class RuleRepository : ActEntityRepositoryBase<Rule, int, ApplicationUser, string>, IRuleRepository
    {

        public RuleRepository(ApplicationDbContext db)
            :base(db)
        {
        }

        public override async Task UpdateAsync(Rule rule) => await UpdateSelectiveAsync<RuleUpdateDTO>(rule);
    }

    public interface IRuleRepository : IActEntityRepository<Rule, int, ApplicationUser, string>
    {
        
    }
}
