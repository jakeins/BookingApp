using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs.Folder;
using BookingApp.Exceptions;
using BookingApp.Repositories.Bases;
using BookingApp.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class FolderRepository
        : ActEntityRepositoryBase<Folder, int, ApplicationUser, string>, 
        IFolderRepository
    {
        public FolderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        private async Task<Folder> GetAsync(int? id)
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

        public override async Task UpdateAsync(Folder Folder)
        {
            await UpdateSelectiveAsync<FolderUpdateSubsetDto>(Folder);
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
                        throw new OperationRestrictedRelationException("Specified parent Folder can't be set because this would cause circluar dependency");
                    else
                    {
                        currentParentId = (await GetAsync(currentParentId)).ParentFolderId;
                    }
                }
            }
        }

    }

}
