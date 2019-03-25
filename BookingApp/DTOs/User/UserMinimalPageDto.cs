using BookingApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs.User
{
    public class UserMinimalPageDto
    {
        public PagingHeader Paging { get; set; }
        public IEnumerable<UserMinimalDto> Items { get; set; }
    }
}
