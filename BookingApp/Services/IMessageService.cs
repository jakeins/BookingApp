using BookingApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public interface IMessageService
    {
        Task SendAsync(Message message);
    }
}
