using System.ComponentModel.DataAnnotations;

namespace BookingApp.DTOs.Folder
{
    public class FolderBaseDto : FolderMinimalDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid FolderId identifier.")]
        public int Id { get; set; }
    }
}
