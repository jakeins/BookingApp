using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleDetailedDTO
    {
        public int Id { get; set; }

        [MaxLength(64, ErrorMessage = "Description is too big.")]
        public string Title { get; set; }

        public int MinTime { get; set; }

        public int MaxTime { get; set; }

        public int StepTime { get; set; }

        public int PreOrderTimeLimit { get; set; }

        public int ReuseTimeout { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        [MaxLength(450)]
        public string CreatedUserId { get; set; }

        [MaxLength(450)]
        public string UpdatedUserId { get; set; }
    }
}
