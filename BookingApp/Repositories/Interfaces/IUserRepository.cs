using BookingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Interfaces
{
    public interface IUserRepository : IBasicRepositoryAsync<ApplicationUser, string>
    {
        Task AddUserRole(ApplicationUser user, string role);
        Task AddUserRoles(ApplicationUser user, IEnumerable<string> roles);
        Task ChangePassword(ApplicationUser user, string currentpassword, string newpassword);
        Task<bool> CheckPassword(ApplicationUser user, string password);
        Task CreateAsync(ApplicationUser user, string password);
        Task DeleteAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetToken(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByUserName(string userName);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<IList<string>> GetUserRolesById(string userId);
        Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName);
        Task<IEnumerable<ApplicationUser>> GetUsersById(IEnumerable<string> usersId);
        Task<int> GetUsersCount();
        Task<IEnumerable<ApplicationUser>> GetListAsync(int page, int pageSize);
        Task<bool> IsInRole(ApplicationUser user, string role);
        Task RemoveUserRole(ApplicationUser user, string role);
        Task RemoveUserRoles(ApplicationUser user, IEnumerable<string> roles);
        Task ResetUserPassword(ApplicationUser user, string token, string newPassword);
    }
}
