using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService
    {
        readonly ResourcesRepository resourcesRepo;

        public ResourcesService(ResourcesRepository resourcesRepo, ApplicationDbContext dbContext)
        {
            this.resourcesRepo = resourcesRepo;
        }

        #region CRUD operations
        public async Task<IEnumerable<Resource>> GetList(bool includeInactives) => await resourcesRepo.GetList(includeInactives);

        public async Task<Resource> Get(int id) => await resourcesRepo.GetAsync(id);

        public async Task Create(Resource item, ApplicationUser creator)
        {
            item.Creator = item.Updater = creator;
            await resourcesRepo.CreateAsync(item);
        }

        public async Task Update(Resource item, ApplicationUser updater)
        {
            item.Creator = item.Updater = updater;
            await resourcesRepo.UpdateAsync(item);
        }

        public async Task Delete(int id) => await resourcesRepo.DeleteAsync(id);
        #endregion

        #region Extended operations

        public async Task<bool> IsResourceActive(int id) => await resourcesRepo.IsResourceActive(id);

        public async Task<double?> GetOccupancy(int resourceId) => await resourcesRepo.GetResourceOccupancy(resourceId);

        public async Task<Dictionary<int, double?>> GetOccupancies(bool includeIncatives)
        {
            var idsList = await resourcesRepo.GetIDsList(includeIncatives);
            var map = new Dictionary<int, double?>();

            foreach (int resourceId in idsList)
            {
                map.Add(resourceId, null);

                try
                {
                    map[resourceId] = await resourcesRepo.GetResourceOccupancy(resourceId);
                }
                catch (KeyNotFoundException)
                {
                }
                catch(AbsurdFieldValueException)
                {
                }
            }
            return map;
        }

        #endregion
    }
}
