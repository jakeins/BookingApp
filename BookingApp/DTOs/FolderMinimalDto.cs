using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.DTOs
{
    public class FolderMinimalDto
    {
        [Required]
        [StringLength(64, MinimumLength = 3, ErrorMessage = "Title should be no more 64 characters")]
        public string Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid parent identifier.")]
        public int? ParentFolderId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid rule identifier.")]
        public int? DefaultRuleId { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsActive { get; set; }
    }
}
