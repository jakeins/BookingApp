using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs.Folder;
using BookingApp.Exceptions;
using BookingApp.Repositories.Bases;
using BookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public override async Task UpdateAsync(Folder Folder)
        {
            await UpdateSelectiveAsync<FolderUpdateSubsetDto>(Folder);
        }

        public async Task IsParentValidAsync(int? newParentId, int currentId)
        {
            await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC [Folders.IsParentValid] {newParentId}, {currentId}");
        }

    }

}
