using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models.Dto
{
    public class UpdateTree
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int? Parent { get; set; }

        public int? Rule { get; set; }

        [Required]
        public string UserCreate { get; set; }

        [Required]
        public string UserUpdate { get; set; }
    }
}
