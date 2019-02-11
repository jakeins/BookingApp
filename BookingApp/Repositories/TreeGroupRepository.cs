using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class TreeGroupRepository : IRepositoryAsync<TreeGroup, int>
    {

        ApplicationDbContext context;

        public TreeGroupRepository(ApplicationDbContext c)
        {
            context = c;
        }

        public async Task<IEnumerable<TreeGroup>> GetListAsync()
        {
            return await context.TreeGroups.ToListAsync();
        }

        public async Task<IEnumerable<TreeGroup>> GetListForUserAsync()
        {
            return await context.TreeGroups.Where(t => t.IsActive == true).ToListAsync();
        }

        public async Task<TreeGroup> GetAsync(int id)
        {
            TreeGroup tree = await context.TreeGroups.FirstOrDefaultAsync(t => t.TreeGroupId == id);
            if (tree != null)
            {
                return tree;
            } else
            {
                throw new CurrentEntryNotFoundException("This TreeGroup does't isset.");
            }
        }

        public async Task CreateAsync(TreeGroup tree)
        {
            context.TreeGroups.Add(tree);
            await SaveAsync();
        }

        public async Task UpdateAsync(TreeGroup tree)
        {
            context.TreeGroups.Update(tree);
            await SaveAsync(); 
        }

        public async Task DeleteAsync(int id)
        {
            TreeGroup tree = await GetAsync(id);
            context.TreeGroups.Remove(tree);
            await SaveAsync();
            
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
