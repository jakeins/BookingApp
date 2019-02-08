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
        public async Task<IEnumerable<Resource>> ListResources(bool includeInactives)
        {
            if(includeInactives)
                return await resourcesRepo.GetListAsync();
            else
                return await resourcesRepo.GetActiveListAsync();
        }

        public async Task<Resource> SingleResource(int id) => await resourcesRepo.GetAsync(id);

        public async Task Create(Resource item) => await resourcesRepo.CreateAsync(item);

        public async Task Update(Resource item) => await resourcesRepo.UpdateAsync(item);

        public async Task Delete(int id) => await resourcesRepo.DeleteAsync(id);
        #endregion

        #region Extended operations

        public async Task<bool> IsActive(int id) => await resourcesRepo.IsActiveAsync(id);

        public async Task<IEnumerable<int>> ListIDsAsync(bool includeIncatives)
        {
            return includeIncatives ? await resourcesRepo.ListIDsAsync() : await resourcesRepo.ListActiveIDsAsync();
        }
        #endregion
    }
}