using BookingApp.Data;
using BookingApp.Data.Models;

namespace BookingApp.Repositories
{
    public class RuleRepository : ActEntityRepositoryBase<Rule, int, ApplicationUser, string>, IRuleRepository
    {

        public RuleRepository(ApplicationDbContext db)
            :base(db)
        {
        }                                                               

    }
}
