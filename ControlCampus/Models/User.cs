using System.ComponentModel.DataAnnotations;

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
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt {  get; set; } = DateTime.Now;

        // Relación con Roles
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
