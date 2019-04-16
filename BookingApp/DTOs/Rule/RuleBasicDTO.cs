using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleBasicDTO
    {
        const string defaultConstraint = @"^[0-9]{1,5}$";            //Only digits 0-9 allowed, and 1 digit minimum

        [Required, RegularExpression(@"^.{4,64}$")]                  //Min size - 4, max size - 64
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
