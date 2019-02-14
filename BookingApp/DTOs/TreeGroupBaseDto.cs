using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class TreeGroupBaseDto : TreeGroupMinimalDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid TreeGroupId identifier.")]
        public int Id { get; set; }

    }
}
