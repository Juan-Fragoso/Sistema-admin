using ControlCampus.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Data
{
    public class Connection : DbContext
    {
        public Connection(DbContextOptions<Connection> options) : base(options)
        {
        }

        // Definimos los DbSet. Cada uno representa una tabla en tu SQL Server.
        // Usa el nombre en singular como tus clases, o plural si prefieres.
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<CourseAssignment> CourseAssignment { get; set; }
        public DbSet<RoleUser> RoleUser { get; set; }

        // Configuración adicional (opcional, para evitar ciclos de cascada)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
