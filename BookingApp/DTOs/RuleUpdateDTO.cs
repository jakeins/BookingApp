using System;
namespace BookingApp.DTOs
{
    public class RuleUpdateDTO : RuleDetailedDTO
    {
        public DateTime UpdatedTime { get; set; }
        public string UpdatedUserId { get; set; }
    }
}
