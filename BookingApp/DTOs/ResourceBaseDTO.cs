using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    /// <summary>
    /// Suitable for writing basic properties. No ID.
    /// </summary>
    public class ResourceBaseDto
    {
        [MinLength(3, ErrorMessage = "Title is too short.")]
        [MaxLength(64, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid group identifier.")]
        public int? FolderId { get; set; }

        [Required]
        public bool? IsActive { get; set; }
    }
}
