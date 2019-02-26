using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public interface IUserService
    {
        Task AddUserRoleAsync(string userId, string role);
        Task AddUsersRoleAsync(ApplicationUser user, IEnumerable<string> roles);
        Task ChangePassword(string userId, string currentpassword, string newpassword);
        Task<bool> CheckPassword(ApplicationUser user, string password);
        Task CreateAdmin(ApplicationUser user, string password);
        Task CreateUser(ApplicationUser user);
        Task CreateUser(ApplicationUser user, string password);
        Task DeleteUser(ApplicationUser user);
        Task DeleteUser(string id);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<ApplicationUser> GetUserById(string id);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<IList<string>> GetUserRolesById(string userId);
        Task<IEnumerable<ApplicationUser>> GetUsersList();
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task RemoveUserRoleAsync(string userId, string role);
        Task RemoveUsersRoleAsync(ApplicationUser user, IEnumerable<string> roles);
        Task ResetUserPassword(string userId, string token, string newPassword);
        Task UpdateUser(ApplicationUser user);
        Task UserApproval(string userId, bool IsApproved);
        Task UserBlocking(string userId, bool IsBlocked);
    }

    public class UserService : IUserService
    {

        private IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task CreateUser(ApplicationUser user)
        {     
            await userRepository.CreateAsync(user);
        }

        public async Task CreateUser(ApplicationUser user, string password)
        {
            await userRepository.CreateAsync(user, password);
        }

        public async Task CreateAdmin(ApplicationUser user, string password)
        {
            user.ApprovalStatus = true;
            await userRepository.CreateAsync(user, password);
        }

        public async Task DeleteUser(string id)
        {
            await userRepository.DeleteAsync(id);
        }

        public async Task DeleteUser(ApplicationUser user)
        {
            await userRepository.DeleteAsync(user);
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            await userRepository.UpdateAsync(user);
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await userRepository.GetAsync(id);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await userRepository.GetUserByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersList()
        {
            return await userRepository.GetListAsync();
        }

        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await userRepository.CheckPassword(user, password);
        }

        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            return await userRepository.GetUserRoles(user);
        }

        public async Task<IList<string>> GetUserRolesById(string userId)
        {
            return await userRepository.GetUserRolesById(userId);
        }

        public async Task ChangePassword(string userId, string currentpassword, string newpassword)
        {
            ApplicationUser user = await GetUserById(userId);
            await userRepository.ChangePassword(user,currentpassword,newpassword);
        }

        public async Task AddUserRoleAsync(string userId,string role)
        {
            ApplicationUser user = await GetUserById(userId);
            await userRepository.AddUserRole(user,role);
        }

        public async Task RemoveUserRoleAsync(string userId, string role)
        {
             ApplicationUser user = await GetUserById(userId);
            await userRepository.RemoveUserRole(user, role);
        }

        public async Task AddUsersRoleAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            await userRepository.AddUserRoles(user, roles);
        }

        public async Task RemoveUsersRoleAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            await userRepository.RemoveUserRoles(user, roles);
        }

        public async Task ResetUserPassword(string userId, string token, string newPassword)
        {
            ApplicationUser user = await GetUserById(userId);
            await userRepository.ResetUserPassword(user, token,newPassword);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return  await userRepository.IsInRole(user, role);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await userRepository.GeneratePasswordResetToken(user);
        }

        public async Task UserApproval(string userId, bool IsApproved)
        {
            ApplicationUser user = await GetUserById(userId);
            user.ApprovalStatus = IsApproved;
            await userRepository.UpdateAsync(user);
        }

        public async Task UserBlocking(string userId, bool IsBlocked)
        {
            ApplicationUser user = await GetUserById(userId);
            user.IsBlocked = IsBlocked;
            await userRepository.UpdateAsync(user);
        }
    }
}
