using Microsoft.EntityFrameworkCore;
using ControlCampus.Models;

namespace ControlCampus.Data
{
    public class AppDBContext:DbContext
    {

        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) {
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col=>col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col=>col.Name);
                tb.Property(col=>col.Email);
                tb.Property(col=>col.Password);
            });

            modelBuilder.Entity<User>().ToTable("User");
        }

    }
}
