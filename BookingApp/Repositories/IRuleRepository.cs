using BookingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public interface IRuleRepository : IActEntityRepository<Rule, int, ApplicationUser, string>
    {
    }
}
