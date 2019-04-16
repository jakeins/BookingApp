using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleBasicDTO
    {    

        [Required]               
        public string Title { get; set; }
    
        public int MinTime { get; set; }

        public int MaxTime { get; set; }

        public int StepTime { get; set; }

        public int ServiceTime { get; set; }

        public int PreOrderTimeLimit { get; set; }

        public int ReuseTimeout { get; set; }

    }
}
