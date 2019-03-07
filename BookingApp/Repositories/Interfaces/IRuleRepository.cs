using BookingApp.Data.Models;

namespace BookingApp.Repositories.Interfaces
{
    public interface IRuleRepository : IActEntityRepository<Rule, int, ApplicationUser, string>
    {
    }
}
