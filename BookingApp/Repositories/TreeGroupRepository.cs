using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
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
            if (tree is TreeGroup)
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
            await GetAsync(tree.TreeGroupId);
            try
            {
                var propsToModify = typeof(TreeGroupMinimalTdo).GetProperties()
                    .Where(prop => prop.Name != "TreeGroupId")
                    .Select(prop => prop.Name)
                    .Concat(new[] { "UpdatedTime", "UpdatedUserId" });

                foreach (var propName in propsToModify)
                    context.Entry(tree).Property(propName).IsModified = true;

                await SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(ex, "TreeGroup Update");
            }
        }

        public async Task DeleteAsync(int id)
        {
            TreeGroup tree = await GetAsync(id);
            context.TreeGroups.Remove(tree);
            try
            {
                await SaveAsync();
            }
            catch (DbUpdateException dbuException)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException, "TreeGroup Delete");
            }          
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();

        
        public async Task IsNotParentAsync(int parentId, int treeId)
        {
            if (parentId == treeId)
            {
                throw new Exception("Current treeGroup can't have this child.");
            }
            if (await IsCurrentChildAsync(treeId, parentId))
            {
                throw new Exception("Current treeGroup can't have this child.");
            }
        }

        private static int count = 0;

        public async Task<bool> IsCurrentChildAsync(int treeId, int parent)
        {
            List<TreeGroup> listTree = await GetChildTree(treeId);
            if (listTree != null)
            {
                count++;
                foreach (TreeGroup tree in listTree)
                {
                    if (tree.TreeGroupId == parent)
                    {
                        return true;
                    }
                    return await IsCurrentChildAsync(tree.TreeGroupId, parent);
                }
            }
            return (count == 1) ? false : true;
        }


            /*
            public async Task<bool> IsCurrentChildAsync(int treeId, int parent)
            {
                List<TreeGroup> listTree = await GetChildTree(treeId);
                if (listTree != null)
                {
                    foreach (TreeGroup tree in listTree)
                    {
                        if (tree.TreeGroupId == parent)
                        {
                            return true;
                        }
                        if (tree.ParentTreeGroupId == parent)
                        {
                            return true;
                        }

                        return await IsCurrentChildAsync(tree.TreeGroupId, parent);
                    }
                }
                return false;
            }
            */

        public async Task<List<TreeGroup>> GetChildTree(int parent)
        {
            return await context.TreeGroups.Where(t => t.ParentTreeGroupId == parent).ToListAsync();
        }

    }
}
