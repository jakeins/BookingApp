using BookingApp.Data;
using BookingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class ResourcesRepository //: IRepository<Resource, int>
    {
        ApplicationDbContext dbContext;

        public ResourcesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Resource>> GetList() => await dbContext.Resources.ToListAsync();

        public async Task<Resource> Get(int id) => await dbContext.Resources.FirstOrDefaultAsync(p => p.ResourceId == id);

        public async Task Create(Resource item)
        {
            dbContext.Resources.Add(item);
            await Save();
        }

        public async Task Update(Resource item)
        {
            dbContext.Resources.Update(item);
            await Save();
        }

        public async Task Delete(int id)
        {
            if (await dbContext.Resources.FirstOrDefaultAsync(p => p.ResourceId == id) is Resource item)
            {
                dbContext.Resources.Remove(item);
                await Save();
            }
        }

        public async Task Save() => await dbContext.SaveChangesAsync();
    }
}
