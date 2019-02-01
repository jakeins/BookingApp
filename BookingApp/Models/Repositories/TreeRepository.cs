using BookingApp.Data;
using BookingApp.Services;
using Microsoft.EntityFrameworkCore;
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
           
            return context.TreeGroups
        .Select(t => new TreeGroup
        {
            TreeGroupId = t.TreeGroupId,
            Title = t.Title,
            ParentTreeGroupId = t.ParentTreeGroupId,
            DefaultRuleId = t.DefaultRuleId,
            CreatedDate = t.CreatedDate,
            UpdatedDate = t.UpdatedDate,
            CreatedUserId = t.CreatedUserId,
            UpdatedUserId = t.UpdatedUserId,
            //Resources = t.Resources,
            //ChildGroups = t.ChildGroups
        })
        .ToList();
            
            
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
