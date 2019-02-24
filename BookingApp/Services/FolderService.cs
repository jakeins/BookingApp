using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BookingApp.Services
{
    public class FolderService
    {
        FolderRepository repository;
        readonly UserManager<ApplicationUser> userManager;

        public FolderService(FolderRepository r, UserManager<ApplicationUser> um)
        {
            repository = r;
            userManager = um;
        }

        public async Task<IEnumerable<Folder>> GetFoldersActive()
        {
            return await repository.ListActiveAsync();
        }

        public async Task<IEnumerable<Folder>> GetFolders()
        {
            return await repository.GetListAsync();
        }

        public async Task<Folder> GetDetail(int id)
        {
            return await repository.GetAsync(id);
        }

        public async Task Create(string userId, Folder Folder)
        {
            Folder.UpdatedUserId = Folder.CreatedUserId = userId;
            await repository.CreateAsync(Folder);
        }

        public async Task Update(int currentFolderId, string userId, Folder Folder)
        {
            await repository.IsParentValidAsync(Folder.ParentFolderId, currentFolderId);    

            Folder.Id = currentFolderId;
            Folder.UpdatedUserId = userId;
            Folder.UpdatedTime = DateTime.Now;
            await repository.UpdateAsync(Folder);
        }

        public async Task Delete(int id) {
            await repository.DeleteAsync(id);
        } 

    }
}
