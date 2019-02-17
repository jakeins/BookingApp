using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class TreeGroupRepository 
        : ActEntityRepositoryBase<TreeGroup, int, ApplicationUser, string>, 
        IActEntityRepository<TreeGroup, int, ApplicationUser, string>
    {
        public TreeGroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        private async Task<TreeGroup> GetAsync(int? id)
        {
            if (id == null)
            {
                throw NewNotFoundException;
            }
            else
            {
                return await base.GetAsync((int)id);
            } 
        }

        public override async Task UpdateAsync(TreeGroup treeGroup)
        {
            await UpdateSelectiveAsync<TreeGroupUpdateSubsetDto>(treeGroup);
        }

        public async Task<bool> IsParentValidAsync(int? newParentId, int? currentId)
        {
            var currentParentId = newParentId;

            while (true)
            {
                if (currentParentId == null)
                    return true;
                else
                {
                    if (currentParentId == currentId)
                        throw new OperationRestrictedRelationException("Specified parent TreeGroup can't be set because this would cause circluar dependency");
                    else
                    {
                        currentParentId = (await GetAsync(currentParentId)).ParentTreeGroupId;
                    }
                }
            }
        }

    }  
}
