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
    }
}
