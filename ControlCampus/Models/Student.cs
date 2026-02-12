using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class Student
    {
        [Key]
        public long? Id { get; set; }
        public string? Phone { get; set; }
        [Required]
        public string Boleta { get; set; } = string.Empty;

        public long UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
