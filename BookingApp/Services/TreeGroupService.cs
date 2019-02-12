using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
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

        public async Task<IEnumerable<TreeGroup>> GetThree(bool isAdmin)
        {
            return (isAdmin) ? await repository.GetListAsync() : await repository.GetListForUserAsync();
        }

        public async Task<TreeGroup> GetDetail(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task Create(TreeGroup tree)
        {
            tree.UpdatedUserId = tree.CreatedUserId = await GetMockUserId();
            await repository.CreateAsync(tree);
        }

        public async Task Update(int id, TreeGroup tree)
        {
            tree.TreeGroupId = id;
            tree.UpdatedUserId = tree.CreatedUserId = await GetMockUserId();
            await repository.UpdateAsync(tree);
        }

        public async Task Delete(int id) {
            await repository.DeleteAsync(id);
        }

       //Mock UserId
        private async Task<string> GetMockUserId()
        {
            ApplicationUser user = await userManager.FindByNameAsync("SuperAdmin");
            return user.Id;
        }

    }
}
