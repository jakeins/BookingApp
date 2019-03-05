using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs
{
    public class FolderBaseDto : FolderMinimalDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid FolderId identifier.")]
        public int Id { get; set; }
    }
}
