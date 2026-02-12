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

            // Configuramos la tabla Grade para evitar ciclos de cascada
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany() // O la colección si la tienes en Teacher
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany()
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Seed de Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin", Description = "Administrador del sistema" },
                new Role { Id = 2, Name = "student", Description = "Cliente del sistema" },
                new Role { Id = 3, Name = "teacher", Description = "Docente del sistema" }
            );

            // Seed de Subjects
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Code = "MAT", Name = "Matemáticas" },
                new Subject { Id = 2, Code = "LEN", Name = "Lengua y Literatura" },
                new Subject { Id = 3, Code = "SOC", Name = "Ciencias Sociales" },
                new Subject { Id = 4, Code = "NAT", Name = "Ciencias Naturales" },
                new Subject { Id = 5, Code = "FIS", Name = "Educación Física" }
            );

            // Seed de Groups
            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "Grupo A", Period = "2026-1" },
                new Group { Id = 2, Name = "Grupo B", Period = "2026-1" },
                new Group { Id = 3, Name = "Grupo C", Period = "2026-1" },
                new Group { Id = 4, Name = "Grupo D", Period = "2026-1" },
                new Group { Id = 5, Name = "Grupo E", Period = "2026-1" }
            );

        }
    }
}
