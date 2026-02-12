using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class CourseAssignment
    {
        [Key]
        public long Id { get; set; }
        public long TeacherId { get; set; }
        public long SubjectId { get; set; }
        public long GroupId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
