using BookingApp.Data.Models;
using BookingApp.Helpers;
using BookingApp.Repositories;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class UserService : IUserService
    {
        private IUserRepository userRepository;
        private readonly INotificationService notificationService;

        public UserService(IUserRepository userRepository, INotificationService notificationService) 
        {
            this.userRepository = userRepository;
            this.notificationService = notificationService;
        }

        public async Task CreateUser(ApplicationUser user)
        {     
            await userRepository.CreateAsync(user);
        }

        public async Task CreateUser(ApplicationUser user, string password)
        {
            await userRepository.CreateAsync(user, password);
        }

        public async Task CreateAdmin(ApplicationUser user)
        {            
            await userRepository.CreateAsync(user, GenerateRandomPassword());
            user = await GetUserByName(user.UserName);//JIC
            await AddUsersRoleAsync(user, new List<string> { RoleTypes.Admin, RoleTypes.User });
            await notificationService.SendPasswordResetNotification(user, await GeneratePasswordResetTokenAsync(user));
            await UserApproval(user.Id, IsApproved: true);
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

        public async Task<IEnumerable<ApplicationUser>> GetUsersById(IEnumerable<string> usersId)
        {
            return await userRepository.GetUsersById(usersId);
        }

        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            return await userRepository.GetUserByUserName(userName);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await userRepository.GetUserByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersList()
        {
            return await userRepository.GetListAsync();
        }

        public async Task<PagedList<ApplicationUser>> GetUsersList(int pageNumber, int pageSize)
        {
            IEnumerable<ApplicationUser> users = await userRepository.GetListAsync(pageNumber, pageSize);
            int countOfUser = await userRepository.GetUsersCount();
            PagedList<ApplicationUser> pagedList = new PagedList<ApplicationUser>(users, pageNumber, pageSize, countOfUser);
            return pagedList;
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

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName)
        {
            return await userRepository.GetUsersByRole(roleName);
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

            if (IsApproved)
                await notificationService.SendApprovalNotification(user);
        }

        public async Task UserBlocking(string userId, bool IsBlocked)
        {
            ApplicationUser user = await GetUserById(userId);
            user.IsBlocked = IsBlocked;
            await userRepository.UpdateAsync(user);
        }

        public async Task RemoveAllRolesFromUser(string userId)
        {
            foreach (var role in await GetUserRolesById(userId))
            {
                await RemoveUserRoleAsync(userId, role);
            }
        }

        public async Task<bool> IsEmailExist(string email)
        {
            if (await GetUserByEmail(email) != null)
                return true;
            else
                return false;
        }

        private  string GenerateRandomPassword(PasswordOptions opts = null)
        {
            string[] randomChars = new[] {
              "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
              "abcdefghijkmnopqrstuvwxyz",    // lowercase
              "0123456789",                   // digits
              "!@$?_-"                        // non-alphanumeric
             };
            opts  =  PasswordSettings.GetPasswordSettings().Password;
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
