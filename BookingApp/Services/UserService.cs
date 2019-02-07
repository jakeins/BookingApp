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
            if (user == null)
                throw new NullReferenceException("Parameter user can not be null");
            await userRepository.CreateAsync(user);
        }
        public async Task CreateUser(ApplicationUser user, string password)
        {
            if (user == null)
                throw new NullReferenceException("Parameter user can not be null");
            else if (password == null)
                throw new NullReferenceException("Parameter password can not be null");
            await userRepository.CreateAsync(user, password);
        }
        public async Task DeleteUser(string id)
        {
            if (id == null)
                throw new ArgumentNullException("Parameter id can not be null");
            else
                await userRepository.DeleteAsync(id);
        }
        public async Task DeleteUser(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException("Parameter user can not be null");
            else
                await userRepository.DeleteAsync(user);
        }
        public async Task UpdateUser(ApplicationUser user)
        {
            if (user == null)
                throw new NullReferenceException("Parameter user can not be null");
            else
                await userRepository.UpdateAsync(user);
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("Parameter id can not be null");
            return await userRepository.GetAsync(id);
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersList()
        {
            return await userRepository.GetListAsync();
        }
        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            if (user == null)
                throw new NullReferenceException("Parameter user can not be null");
            else if (password == null)
                throw new NullReferenceException("Parameter password can not be null");
            return await userRepository.CheckPassword(user, password);
        }
        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            if (user == null)
                throw new NullReferenceException("Parameter user can not be null");
            return await userRepository.GetUserRoles(user);
        }
        public async Task<IList<string>> GetUserRolesById(string userId)
        {
            if (userId == null)
                throw new NullReferenceException("Parameter userId can not be null");
            return await userRepository.GetUserRolesById(userId);
        }
    }
}
