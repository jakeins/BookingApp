using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class RuleBasicDTO
    {
        const string title_regex = "[A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії'0-9- _]+";
        [Required, MaxLength(64, ErrorMessage = "Max length of title is 64"), MinLength(4, ErrorMessage = "Min Length of title is 4"), RegularExpression(title_regex, ErrorMessage ="Incorrect title")]               
        public string Title { get; set; }
    
        [Required, Range(0, 14400, ErrorMessage = "Min time can't be lower than  0 and equal or be greater 14400")]
        public int MinTime { get; set; }

        [Required, Range(0, 14400, ErrorMessage = "Max time can't be lower than  0 and equal or be greater 14400")]
        public int MaxTime { get; set; }

        [Required, Range(1, 14400, ErrorMessage = "Step time can't be lower than  1 and equal or be greater 14400")]
        public int StepTime { get; set; }

        [Required, Range(0, 14400, ErrorMessage = "Service time can't be lower than  0 and equal or be greater 14400")]
        public int ServiceTime { get; set; }

        [Required, Range(0, 14400, ErrorMessage = "Reuse timeout time can't be lower than  0 and equal or be greater 14400")]
        public int PreOrderTimeLimit { get; set; }

        [Required, Range(0, 14400, ErrorMessage = "Pre order time limit can't be lower than  0 and equal or be greater 14400")]
        public int ReuseTimeout { get; set; }

    }
}
