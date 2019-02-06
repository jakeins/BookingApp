using BookingApp.Data;
using BookingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<TreeGroup> GetAsync(int id)
        {
            return await context.TreeGroups.FirstOrDefaultAsync(t => t.TreeGroupId == id);
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
            List<TreeGroup> children = await context.TreeGroups.Where(t => t.ParentTreeGroupId == id).ToListAsync();
            if (children != null)
            {
                foreach (TreeGroup child in children)
                {
                    child.ParentTreeGroupId = null;
                    context.TreeGroups.Update(child);
                    await SaveAsync();
                }
            }    

            TreeGroup tree = await context.TreeGroups.FirstOrDefaultAsync(p => p.TreeGroupId == id);
            if (tree != null)
            {
                context.TreeGroups.Remove(tree);
                await SaveAsync();
            }
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();

    }
}
