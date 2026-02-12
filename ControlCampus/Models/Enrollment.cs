using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class Enrollment
    {
        [Key]
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long GroupId { get; set; }
        public string Status { get; set; } = "enrolled";

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; } = null!;
    }
}
