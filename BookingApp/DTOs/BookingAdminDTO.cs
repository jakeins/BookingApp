using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class BookingAdminDTO : BookingOwnerDTO
    {
        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
