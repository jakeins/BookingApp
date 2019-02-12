using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class TreeGroupBaseDto : TreeGroupMinimalTdo
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid TreeGroupId identifier.")]
        public int TreeGroupId { get; set; }

    }
}
