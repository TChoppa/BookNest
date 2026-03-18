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
            public DbSet<Book> Books { get; set; }
            public DbSet<Cart> CartList { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderItem> OrderItems {  get; set; }
            public DbSet<IssuedBook> IssuedBooks { get; set; }
            public DbSet<ClubHostRoom> ClubHostRooms { get; set; }
            public DbSet<ClubHostMember> ClubHostMembers { get; set; }
            public DbSet<ClubHostAttachment> ClubHostAttachments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder); // always keep this

                modelBuilder.Entity<User>()
                    .HasOne(u => u.RoleMaster)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.Fk_RoleId);

                modelBuilder.Entity<ClubHostMember>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Members)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

                // Room -> Attachments (1-to-many)
                modelBuilder.Entity<ClubHostAttachment>()
                    .HasOne(a => a.Room)
                    .WithMany(r => r.Attachments)
                    .HasForeignKey(a => a.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
        }



        }
    }
