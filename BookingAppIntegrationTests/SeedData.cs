using BookingApp.Data;
using BookingApp.Data.Models;


namespace BookingAppIntegrationTests
{
    public static class SeedData
    {
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            dbContext.Folders.Add(new Folder { Id = 1, Title = "Folder 1" });
            dbContext.Folders.Add(new Folder { Id = 2, Title = "Folder 2" });
            dbContext.SaveChanges();
        }
    }
}
