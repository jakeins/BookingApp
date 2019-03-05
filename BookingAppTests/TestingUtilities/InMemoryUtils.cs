using BookingApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;

namespace TestingUtilities
{
    public class InMemoryUtils
    {
        public static DbContextOptions<ApplicationDbContext> ProduceFreshDbContextOptions([CallerMemberName] string caller = "")
        {
            string name = "TestBase" + Guid.NewGuid().ToString() + " " + caller;
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
        }
    }
}