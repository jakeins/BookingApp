using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    public class UserPagingParamsDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
