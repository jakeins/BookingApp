using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repositories
{
    public class BookingRepository : Services.IRepositoryAsync<Models.Booking, int>
    {
        Data.ApplicationDbContext dbContext;

        BookingRepository(Data.ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreateAsync(Booking model)
        {
            try
            {
                dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC BookingCreate {model.ResourceId} '{model.CreatedTime}' '{model.EndTime}' '{model.CreatedUserId}' '{model.Note}'");

            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001)
                {
                    switch (ex.State)
                    {
                        //Invalid user id
                        case 1:
                            throw new Exception(ex.Message);
                        //StartTime must be lower than EndTime
                        case 2:
                            throw new Exception(ex.Message);
                        //Invalid resource id passed
                        case 3:
                            
                        //Booking duration less than min valid for this resource
                        case 4:
                        //Booking duration more than max valid for this resource
                        case 5:
                        //The duration of the reservation must be a multiple step of the booking for this resource
                        case 6:
                        //Time range alredy booked
                        case 7:
                        //Other
                        default:
                            break;

                    }
                }
            }
        }

        public void DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Booking>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public void SaveAsync()
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(Booking model)
        {
            throw new NotImplementedException();
        }
    }
}
