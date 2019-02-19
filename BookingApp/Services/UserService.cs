using BookingApp.Data.Models;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class UserService
    {
        private UserRepository userRepository;
        public UserService(UserRepository userRepository)
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

        public async Task ChangePassword(ApplicationUser user, string currentpassword, string newpassword)
        {
            await userRepository.ChangePassword(user,currentpassword,newpassword);
        }

        public async Task AddUserRoleAsync(ApplicationUser user,string role)
        {
            await userRepository.AddUserRole(user,role);
        }

        public async Task RemoveUserRoleAsync(ApplicationUser user, string role)
        {
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

        public async Task RessetUserPassword(ApplicationUser user, string token, string newPassword)
        {
            await userRepository.RessetUserPassword(user, token,newPassword);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return  await userRepository.IsInRole(user, role);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await userRepository.GeneratePasswordResetToken(user);
        }
    }
}
