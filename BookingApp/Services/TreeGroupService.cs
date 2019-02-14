using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BookingApp.Services
{
    public class TreeGroupService
    {
        TreeGroupRepository repository;
        readonly UserManager<ApplicationUser> userManager;

        public TreeGroupService(TreeGroupRepository r, UserManager<ApplicationUser> um)
        {
            repository = r;
            userManager = um;
        }

        public async Task<IEnumerable<TreeGroup>> GetTree(bool isAdmin)
        {
            return (isAdmin) ? await repository.GetListAsync() : await repository.GetListForUserAsync();
        }

        public async Task<TreeGroup> GetDetail(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task Create(string userId, TreeGroup tree)
        {
            tree.UpdatedUserId = tree.CreatedUserId = userId;
            await repository.CreateAsync(tree);
        }

        public async Task Update(int currentTreeGroupId, string userId, TreeGroup tree)
        {
            await repository.IsParentValidAsync(tree.ParentTreeGroupId, currentTreeGroupId);    

            tree.TreeGroupId = currentTreeGroupId;
            tree.UpdatedUserId = userId;
            tree.UpdatedTime = DateTime.Now;
            await repository.UpdateAsync(tree);
        }

        public async Task Delete(int id) {
            await repository.DeleteAsync(id);
        } 

    }
}
