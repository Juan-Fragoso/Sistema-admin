using ControlCampus.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlCampus.Data
{
    public class Connection : DbContext
    {
        public Connection(DbContextOptions<Connection> options) : base(options)
        {
        }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany() 
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany()
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin", Description = "Administrador del sistema" },
                new Role { Id = 2, Name = "student", Description = "Cliente del sistema" },
                new Role { Id = 3, Name = "teacher", Description = "Docente del sistema" }
            );

            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Code = "MAT", Name = "Matemáticas" },
                new Subject { Id = 2, Code = "LEN", Name = "Lengua y Literatura" },
                new Subject { Id = 3, Code = "SOC", Name = "Ciencias Sociales" },
                new Subject { Id = 4, Code = "NAT", Name = "Ciencias Naturales" },
                new Subject { Id = 5, Code = "FIS", Name = "Educación Física" }
            );

            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "Grupo A", Period = "2026-1" },
                new Group { Id = 2, Name = "Grupo B", Period = "2026-1" },
                new Group { Id = 3, Name = "Grupo C", Period = "2026-1" },
                new Group { Id = 4, Name = "Grupo D", Period = "2026-1" },
                new Group { Id = 5, Name = "Grupo E", Period = "2026-1" }
            );

            var fechaFija = new DateTime(2026, 01, 12);

            // Este es el hash de "admin123" generado previamente. 
            string staticHash = "$2a$11$SAZGNCx7o8A2IjWfJEBbwORDzAtvmV5qNiNyBe/nacO5X77lR3ikO";

            modelBuilder.Entity<User>().HasData(new
            {
                Id = (long)1,
                Name = "Administrador",
                Email = "admin@campus.com",
                Password = staticHash,
                CreatedAt = (DateTime?)fechaFija,
                UpdatedAt = (DateTime?)fechaFija
            });

            modelBuilder.Entity<RoleUser>().HasData(new
            {
                Id = (long)1,
                UserId = (long)1,
                RoleId = (long)1,
                CreatedAt = (DateTime?)fechaFija,
                UpdatedAt = (DateTime?)fechaFija
            });

        }
    }
}
