using System.ComponentModel.DataAnnotations;

namespace ControlCampus.Models
{
    public class Role
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
