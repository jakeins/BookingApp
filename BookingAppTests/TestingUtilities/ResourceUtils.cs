using BookingApp.Data.Models;
using System.Collections.Generic;

namespace TestingUtilities
{
    public class ResourceUtils
    {
        static IEnumerable<Resource> testResources;
        public static IEnumerable<Resource> TestSet => testResources ?? FormTestResources();
        static IEnumerable<Resource> FormTestResources()
        {
            testResources = new[]
            {
                  new Resource() { Id = 1, Title = "Nothern View",  IsActive = true, RuleId = 1, Description = "Oculus Diabolus",
                      CreatedUserId = VincentVanGogh, UpdatedUserId = VincentVanGogh },
                  new Resource() { Id = 2, Title = "Southern View", IsActive = true,
                      CreatedUserId = GustaveEiffel, UpdatedUserId = VincentVanGogh },
                  new Resource() { Id = 3, Title = "Flag", IsActive = true,
                      CreatedUserId = VincentVanGogh, UpdatedUserId = GustaveEiffel },

                  new Resource() { Id = 4, Title = "Trumpet Ensemble", IsActive = true },

                  new Resource() { Id = 5, Title = "First Floor Hallway", IsActive = true },

                  new Resource() { Id = 6, Title = "Natural Museum", IsActive = false },
                  new Resource() { Id = 7, Title = "Art Museum", IsActive = false },
                  new Resource() { Id = 8, Title = "History Museum", IsActive = true },

                  new Resource() { Id = 9, Title = "Civil Defence Alarm", IsActive = true },

                  new Resource() { Id = 10, Title = "Cruiser Bicycle #2000", IsActive = true },
                  new Resource() { Id = 11, Title = "Cruiser Bicycle #46", IsActive = true },
                  new Resource() { Id = 12, Title = "Ukraine Tier0 Bicycle", IsActive = true },
                  new Resource() { Id = 13, Title = "Mountain Bike Roger", IsActive = false, Description = "Petro Deus",
                      CreatedUserId = GustaveEiffel, UpdatedUserId = GustaveEiffel },
            };
            return testResources;
        }

        public static string VincentVanGogh => "Vincent van Gogh";
        public static string GustaveEiffel => "Gustave Eiffel";
        
        public static int NonExistentId => 100500;
    }
}