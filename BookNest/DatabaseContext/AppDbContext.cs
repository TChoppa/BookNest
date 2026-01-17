    using BookNest.Models;
    using Microsoft.EntityFrameworkCore;

    namespace BookNest.DatabaseContext
    {
        public class AppDbContext:DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
            {

            }
            public DbSet<User> users { get; set; }
            public DbSet<RoleMaster> RoleMasters { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder); // always keep this

                modelBuilder.Entity<User>()
                    .HasOne(u => u.RoleMaster)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.Fk_RoleId);
            }


        }
    }
