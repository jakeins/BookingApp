using BookingApp.Data.Models;
using BookingApp.Exceptions;
using BookingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
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
            IdentityResult result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task CreateAsync(ApplicationUser user, string password)
        {
            if (userManager.FindByEmailAsync(user.Email) != null)
                throw new UserException("User with this email already registered");
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
                {
                    GetExceptionIdentityResult(result);
                }
            }
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            IdentityResult result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task<ApplicationUser> GetAsync(string userid)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(userid);
            if (applicationUser == null)
            {
                throw new NullReferenceException("Can not find user with this id");
            }
            else
            {
                return applicationUser;
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = await userManager.FindByEmailAsync(email);
            if (applicationUser == null)
            {
                throw new NullReferenceException("Can not find user with this email");
            }
            else
            {
                return applicationUser;
            }
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            ApplicationUser applicationUser = await userManager.FindByNameAsync(userName);
            if (applicationUser == null)
            {
                throw new NullReferenceException("Can not find user with this UserName");
            }
            else
            {
                return applicationUser;
            }
        }

        public Task<IEnumerable<ApplicationUser>> GetListAsync()
        {
            return Task.FromResult(userManager.Users.ToList().AsEnumerable());
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task ChangePassword(ApplicationUser user, string currentpassword,string newpassword)
        {
           IdentityResult result =  await userManager.ChangePasswordAsync(user, currentpassword,newpassword);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        private void GetExceptionIdentityResult(IdentityResult identityResult)
        {
            foreach (IdentityError item in identityResult.Errors)
            {
                switch (item.Code)
                {
                    case "DuplicateUserName":
                        throw new UserException("User with this Username already created");
                    case "DuplicateEmail":
                        throw new UserException("Email already registered");
                    case "InvalidEmail":
                        throw new UserException("Invalid email");
                    case "InvalidUserName":
                        throw new UserException("Invalid Username");
                    case "PasswordTooShort":
                        throw new UserException("Password too short");
                    case "PasswordRequiresNonAlphanumeric":
                        throw new UserException("Password requires non alphanumeric");
                    case "PasswordRequiresDigit":
                        throw new UserException("Password requires digit");
                    case "PasswordRequiresLower":
                        throw new UserException("Password requires lower");
                    case "PasswordRequiresUpper":
                        throw new UserException("Password requires upper");
                    default:
                        throw new UserException("Default User Exception");
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
            {
                throw new NullReferenceException("Can not find user with this id");
            }
            return await userManager.GetRolesAsync(applicationUser);
        }

        public async Task AddUserRole(ApplicationUser user,string role)
        {
          IdentityResult result =   await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task RemoveUserRole(ApplicationUser user, string role)
        {
           IdentityResult result = await userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task AddUserRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            IdentityResult result = await userManager.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task RemoveUserRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            IdentityResult result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task ResetUserPassword(ApplicationUser user,string token,string newPassword)
        {
            IdentityResult result = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                GetExceptionIdentityResult(result);
            }
        }

        public async Task<bool> IsInRole(ApplicationUser user, string role)
        {
            return await userManager.IsInRoleAsync(user, role);          
        }

        public async Task<string> GeneratePasswordResetToken(ApplicationUser user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
