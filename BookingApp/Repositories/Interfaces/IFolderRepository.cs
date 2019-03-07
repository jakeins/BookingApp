using System.Threading.Tasks;
using BookingApp.Data.Models;

namespace BookingApp.Repositories.Interfaces
{
    public interface IFolderRepository
        : IActEntityRepository<Folder, int, ApplicationUser, string>
    {
        Task<bool> IsParentValidAsync(int? newParentId, int? currentId);
    }
}