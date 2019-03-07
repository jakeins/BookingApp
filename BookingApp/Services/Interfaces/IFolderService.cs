using BookingApp.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services.Interfaces
{  
    public interface IFolderService
    {
        Task Create(string userId, Folder Folder);
        Task<IEnumerable<Folder>> GetFoldersActive();
        Task<IEnumerable<Folder>> GetFolders();
        Task<Folder> GetDetail(int folderId);
        Task Update(int currentFolderId, string userId, Folder Folder);
        Task Delete(int folderId);
    }
}
