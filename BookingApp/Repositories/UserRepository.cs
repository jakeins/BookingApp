using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{

    public class UserRepository : IUserRepository
    {
        private UserManager<ApplicationUser> userManager;

        public UserRepository(UserManager<ApplicationUser> user)
        {
            this.userManager = user;
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            EnsureIdentitySuccess(await userManager.CreateAsync(user));
        }

        public async Task CreateAsync(ApplicationUser user, string password)
        {
            if (await userManager.FindByEmailAsync(user.Email) != null)
                throw new UserException("User with this email is already registered");

            EnsureIdentitySuccess(await userManager.CreateAsync(user, password));
        }

        public async Task DeleteAsync(string id)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
            if (applicationUser == null)
                throw new CurrentEntryNotFoundException("There is no User with such UserID to delete.");
            else
            {
                try
                {
                    EnsureIdentitySuccess(await userManager.DeleteAsync(applicationUser));
                }
                catch (DbUpdateException dbuException)
                {
                    Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException, $"User deletion");
                }
            }
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            EnsureIdentitySuccess(await userManager.DeleteAsync(user));
        }

        public async Task<ApplicationUser> GetAsync(string userid)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(userid);
            if (applicationUser == null)
                throw new CurrentEntryNotFoundException("There is no such UserID to get the User from.");
            else
                return applicationUser;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = await userManager.FindByEmailAsync(email);
            if (applicationUser == null)
                throw new CurrentEntryNotFoundException("There is no User with such Email to get.");
            else
                return applicationUser;
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            ApplicationUser applicationUser = await userManager.FindByNameAsync(userName);
            if (applicationUser == null)
                throw new CurrentEntryNotFoundException("There is no User with such UserName to get.");
            else
                return applicationUser;
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersById(IEnumerable<string> usersId)
        {
           return Task.FromResult(userManager.Users.Where(user => usersId.Contains(user.Id)).ToList().AsEnumerable());
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName)
        {
           return await userManager.GetUsersInRoleAsync(roleName);
        }

        public Task<IEnumerable<ApplicationUser>> GetListAsync()
        {
            return Task.FromResult(userManager.Users.ToList().AsEnumerable());
        }

        public Task<IEnumerable<ApplicationUser>> GetListAsync(int pageNumber,int pageSize)
        { 
            return Task.FromResult(userManager.Users.Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .ToList().AsEnumerable());
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException("Save is not applicable for current repository implementation.");
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            EnsureIdentitySuccess(await userManager.UpdateAsync(user));
        }

        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task ChangePassword(ApplicationUser user, string currentpassword,string newpassword)
        {
            EnsureIdentitySuccess(await userManager.ChangePasswordAsync(user, currentpassword, newpassword));
        }

 

        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<IList<string>> GetUserRolesById(string userId)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(userId);
            if (applicationUser == null)
                throw new CurrentEntryNotFoundException("There is no User ID to get the roles from.");
            else
                return await userManager.GetRolesAsync(applicationUser);
        }

        public async Task AddUserRole(ApplicationUser user,string role)
        {
            EnsureIdentitySuccess(await userManager.AddToRoleAsync(user, role));
        }

        public async Task RemoveUserRole(ApplicationUser user, string role)
        {
            EnsureIdentitySuccess(await userManager.RemoveFromRoleAsync(user, role));
        }

        public async Task AddUserRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            EnsureIdentitySuccess(await userManager.AddToRolesAsync(user, roles));
        }

        public async Task RemoveUserRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            EnsureIdentitySuccess(await userManager.RemoveFromRolesAsync(user, roles));
        }

        public async Task ResetUserPassword(ApplicationUser user,string token,string newPassword)
        {
            EnsureIdentitySuccess( await userManager.ResetPasswordAsync(user, token, newPassword) );
        }

        public async Task<bool> IsInRole(ApplicationUser user, string role)
        {
            return await userManager.IsInRoleAsync(user, role);          
        }

        public async Task<string> GeneratePasswordResetToken(ApplicationUser user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<int> GetUsersCount()
        {
            return Task.FromResult(userManager.Users.Count());
        }

        #region Utilities
        private void EnsureIdentitySuccess(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
                return;

            List<string> exceptions = new List<string>();

            foreach (IdentityError item in identityResult.Errors)
            {
                exceptions.Add(item.Code);
            }

            throw new UserException("Identity issue(s): " + string.Join(", ", exceptions));
        }
        #endregion
    }
}
