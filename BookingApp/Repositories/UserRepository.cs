using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingApp.Services
{

    public class UserRepository : IRepositoryAsync<ApplicationUser, string>
    {
        private UserManager<ApplicationUser> userManager;

        public UserRepository(UserManager<ApplicationUser> user)
        {
            this.userManager = user;
        }
        public async Task CreateAsync(ApplicationUser user)
        {
            IdentityResult result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }
        public async Task CreateAsync(ApplicationUser user, string password)
        {
            IdentityResult result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }
        public async Task DeleteAsync(string id)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
            if (applicationUser == null)
                throw new NullReferenceException("Can not find user with this id");
            else
            {
                IdentityResult result = await userManager.DeleteAsync(applicationUser);
                if (!result.Succeeded)
                    GetExceptionIdentityResult(result);
            }
        }
        public async Task DeleteAsync(ApplicationUser user)
        {
            IdentityResult result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                GetExceptionIdentityResult(result);
        }
        public async Task<ApplicationUser> GetAsync(string userid)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(userid);
            if (applicationUser == null)
                throw new NullReferenceException("Can not find user with this id");
            else
                return applicationUser;
        }
        public async Task<IEnumerable<ApplicationUser>> GetListAsync()
        {
            return userManager.Users.ToList();
        }
        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
        public async Task UpdateAsync(ApplicationUser user)
        {
            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                GetExceptionIdentityResult(result);
        }
        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        private void GetExceptionIdentityResult(IdentityResult identityResult)
        {
            foreach (IdentityError item in identityResult.Errors)
            {
                switch (item.Code)
                {
                    case "DuplicateUserName":
                        throw new DuplicateUserNameException("User with this Username already created");
                    case "DuplicateEmail":
                        throw new DuplicateEmailException("Email already registered");
                    case "InvalidEmail":
                        throw new InvalidEmailException("Invalid email");
                    case "InvalidUserName":
                        throw new InvalidUserNameException("Invalid Username");
                    case "PasswordTooShort":
                        throw new PasswordTooShortException("Password too short");
                    case "PasswordRequiresNonAlphanumeric":
                        throw new PasswordRequiresNonAlphanumericException("Password requires non alphanumeric");
                    case "PasswordRequiresDigit":
                        throw new PasswordRequiresDigitException("Password requires digit");
                    case "PasswordRequiresLower":
                        throw new PasswordRequiresLowerException("Password requires lower");
                    case "PasswordRequiresUpper":
                        throw new PasswordRequiresUpperException("Password requires upper");
                    default:
                        throw new DefaultUserException("Default User Exception");
                }
            }
        }
        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user);
        }
        public async Task<IList<string>> GetUserRolesById(string userId)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(userId);
            if (applicationUser == null)
                throw new NullReferenceException("Can not find user with this id");
            return await userManager.GetRolesAsync(applicationUser);
        }
    }
}
