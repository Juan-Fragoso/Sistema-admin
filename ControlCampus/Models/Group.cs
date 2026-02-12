using System.ComponentModel.DataAnnotations;

namespace ControlCampus.Models
{
    public class Group
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Period { get; set; } = string.Empty;
    }
}
