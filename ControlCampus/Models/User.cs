using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCampus.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
       
        public DateTime? CreatedAt { get; set; }
       
        public DateTime? UpdatedAt {  get; set; }

        public virtual Student? Student { get; set; }

        // 1. Relación HasOne (Un usuario tiene un perfil de profesor)
        public virtual Teacher? Teacher { get; set; }

        // Relación con Roles
        public virtual ICollection<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();

        public bool IsAdmin()
        {
            return RoleUsers != null && RoleUsers.Any(ru => ru.Role?.Name == "admin");
        }
    }
}
