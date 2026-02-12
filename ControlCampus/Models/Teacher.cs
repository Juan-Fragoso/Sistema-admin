using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class Teacher
    {
        [Key]
        public long? Id { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long UserId { get; set; }

        public User? User { get; set; } = null!;
    }
}
