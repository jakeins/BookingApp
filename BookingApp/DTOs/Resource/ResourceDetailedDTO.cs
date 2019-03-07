using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs.Resource
{
    /// <summary>
    /// Suitable for detailed writing. No ID.
    /// </summary>
    public class ResourceDetailedDto : ResourceBaseDto
    {
        [MaxLength(512, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid rule identifier.")]
        public int RuleId { get; set; }
    }
}
