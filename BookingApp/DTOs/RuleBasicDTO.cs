using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleBasicDTO
    {
        [MaxLength(64, ErrorMessage = "Description is too big.")]
        public string Title { get; set; }

        public int MinTime { get; set; }

        public int MaxTime { get; set; }

        public int StepTime { get; set; }

        public int PreOrderTimeLimit { get; set; }

        public int ReuseTimeout { get; set; }

    }
}
