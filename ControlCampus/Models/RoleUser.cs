using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class RoleUser
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;

    }
}
