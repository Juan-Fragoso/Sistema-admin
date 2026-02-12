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

        public virtual Student? Student { get; set; }

        // 1. Relación HasOne (Un usuario tiene un perfil de profesor)
        public virtual Teacher? Teacher { get; set; }

        // Relación con Roles
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        // 3. Método isAdmin() equivalente
        // Usamos LINQ para buscar dentro de la colección de Roles
        public bool IsAdmin()
        {
            return Roles != null && Roles.Any(r => r.Name == "admin");
        }

    }
}
