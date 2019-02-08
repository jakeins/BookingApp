using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class BookingCreateDTO : BookingMinimalDTO
    {
        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Note { get; set; }
    }
}
