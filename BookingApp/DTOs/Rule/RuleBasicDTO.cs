using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleBasicDTO
    {
        const string defaultConstraint = @"^[0-9]{1,}$";            

        [Required, MinLength(4, ErrorMessage = "Description is too small."), MaxLength(64, ErrorMessage = "Description is too big.")]
        public string Title { get; set; }

        [Required, RegularExpression(defaultConstraint)]
        public int MinTime { get; set; }

        [Required,RegularExpression(defaultConstraint)]
        public int MaxTime { get; set; }

        [RegularExpression(defaultConstraint)]
        public int StepTime { get; set; }

        [RegularExpression(defaultConstraint)]
        public int ServiceTime { get; set; }

        [RegularExpression(defaultConstraint)]
        public int PreOrderTimeLimit { get; set; }

        [RegularExpression(defaultConstraint)]
        public int ReuseTimeout { get; set; }

    }
}
