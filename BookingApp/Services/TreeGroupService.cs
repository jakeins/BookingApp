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

        public async Task<IEnumerable<TreeGroup>> GetTreeGroupsActive()
        {
            return await repository.ListActiveAsync();
        }

        public async Task<IEnumerable<TreeGroup>> GetTreeGroups()
        {
            return await repository.GetListAsync();
        }

        public async Task<TreeGroup> GetDetail(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task Create(string userId, TreeGroup treeGroup)
        {
            treeGroup.UpdatedUserId = treeGroup.CreatedUserId = userId;
            await repository.CreateAsync(treeGroup);
        }

        public async Task Update(int currentTreeGroupId, string userId, TreeGroup treeGroup)
        {
            await repository.IsParentValidAsync(treeGroup.ParentTreeGroupId, currentTreeGroupId);    

            treeGroup.Id = currentTreeGroupId;
            treeGroup.UpdatedUserId = userId;
            treeGroup.UpdatedTime = DateTime.Now;
            await repository.UpdateAsync(treeGroup);
        }

        public async Task Delete(int id) {
            await repository.DeleteAsync(id);
        } 

    }
}
