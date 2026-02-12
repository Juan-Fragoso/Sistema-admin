using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class Grade
    {
        [Key]
        public long Id { get; set; }
        public long EnrollmentId { get; set; }
        public long SubjectId { get; set; }
        public long TeacherId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? GradeValue { get; set; }
        public string? Term { get; set; }

        [ForeignKey("EnrollmentId")]
        public virtual Enrollment Enrollment { get; set; } = null!;
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
    }
}
