using BookingApp.Data;
using BookingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models.Repositories
{
    public class TreeRepository : IRepository<TreeGroup, int>
    {

        ApplicationDbContext context;

        public TreeRepository(ApplicationDbContext c)
        {
            context = c;
        }

        public IEnumerable<TreeGroup> GetList()
        {
            return context.TreeGroups.ToList();
        }

        public TreeGroup Get(int id)
        {
            return context.TreeGroups.FirstOrDefault(p => p.TreeGroupId == id);
        }

        public void Delete(int id)
        {
            TreeGroup tree = context.TreeGroups.FirstOrDefault(p => p.TreeGroupId == id);
            if (tree != null)
            {
                context.TreeGroups.Remove(tree);
                context.SaveChangesAsync();
            }
        }
    }
}
