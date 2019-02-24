using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleDetailedDTO : RuleBasicDTO
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
