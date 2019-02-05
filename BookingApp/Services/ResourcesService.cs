using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ResourcesService
    {
        readonly ResourcesRepository repository;
        readonly UserManager<ApplicationUser> userManager;

        public ResourcesService(ResourcesRepository repository, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<Resource>> GetList() => await repository.GetList();

        public async Task<Resource> Get(int id) => await repository.Get(id);

        public async Task Create(Resource item)
        {
            item.Creator = item.Updater = await GetCurrentUser();

            await repository.Create(item);
        }

        public async Task Update(Resource item)
        {
            item.Creator = item.Updater = await GetCurrentUser();

            await repository.Update(item);
        }

        public async Task Delete(int id) => await repository.Delete(id);

        public async Task<double> GetOccupancy(int resourceId) => await repository.GetOccupancy(resourceId);

        async Task<ApplicationUser> GetCurrentUser()
        {
            return await userManager.FindByNameAsync("SuperAdmin");//stub
        }
    }
}
