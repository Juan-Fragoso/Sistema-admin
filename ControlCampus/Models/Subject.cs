using System.ComponentModel.DataAnnotations;

namespace ControlCampus.Models
{
    public class Subject
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        public int? Credits { get; set; }
    }
}
