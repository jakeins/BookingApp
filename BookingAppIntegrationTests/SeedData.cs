using BookingApp.Data;
using BookingApp.Data.Models;
using System.Collections.Generic;


namespace BookingAppIntegrationTests
{
    public static class SeedData
    {
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            List<Folder> Folders = new List<Folder>();
            Folders.Add(new Folder { Id = 1, Title = "Folder 1", IsActive = true });
            Folders.Add(new Folder { Id = 2, Title = "Folder 2", IsActive = false });
            Folders.Add(new Folder { Id = 3, Title = "Folder 3", IsActive = true });

            dbContext.Folders.AddRange(Folders);
            dbContext.SaveChanges();
        }
    }
}
