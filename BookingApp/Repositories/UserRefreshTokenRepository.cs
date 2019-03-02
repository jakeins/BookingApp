using BookingApp.Data;
using BookingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public interface IUserRefreshTokenRepository : IBasicRepositoryAsync<UserRefreshToken, int>
    {
        Task<UserRefreshToken> GetByUserIdAsync(string userId);
    }

    public class UserRefreshTokenRepository : IUserRefreshTokenRepository
    {
        private readonly ApplicationDbContext db;

        public UserRefreshTokenRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task CreateAsync(UserRefreshToken model)
        {
            await db.UserRefreshTokens.AddAsync(model); //Create stored procedure
        }

        public Task DeleteAsync(int key)
        {
            throw new NotImplementedException();
        }

        public Task<UserRefreshToken> GetAsync(int key)
        {
            throw new NotImplementedException();
        }

        public async Task<UserRefreshToken> GetByUserIdAsync(string userId)
        {
            return db.UserRefreshTokens.FirstOrDefault(token => token.UserId == userId); 
        }

        public Task<IEnumerable<UserRefreshToken>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserRefreshToken model)
        {
            db.UserRefreshTokens.Update(model); //Create stored procedure
            await SaveAsync();
        }
    }
}
