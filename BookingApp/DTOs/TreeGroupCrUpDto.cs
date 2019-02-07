using System;
using System.ComponentModel.DataAnnotations;


namespace BookingApp.DTOs
{
    public class TreeGroupCrUpDto
    {

        [Required]
        [StringLength(64, MinimumLength = 3, ErrorMessage = "Title should be no more 64 characters")]
        public string Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid parent identifier.")]
        public int? ParentTreeGroupId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid rule identifier.")]
        public int? DefaultRuleId { get; set; }

        [Required]
        [MaxLength(450, ErrorMessage = "No data about User.")]
        public string CreatedUserId { get; set; }

        [Required]
        [MaxLength(450, ErrorMessage = "No data about User.")]
        public string UpdatedUserId { get; set; }

    }
}
