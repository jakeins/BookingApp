using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Data.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [MaxLength(44)]
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
