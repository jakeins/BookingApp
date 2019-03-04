using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleAdminDTO : RuleDetailedDTO
    {
        public int Id { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
