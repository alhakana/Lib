using GET.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GET.Data
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Author).IsRequired();
                entity.Property(e => e.TotalCopies).IsRequired();
                entity.Property(e => e.AvailableCopies).IsRequired();
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReservationDate).IsRequired();
                entity.Property(e => e.DueDate).IsRequired();
                entity.Property(e => e.IsReturned).IsRequired();

                entity.HasOne(e => e.Book)
                      .WithMany(b => b.Reservations)
                      .HasForeignKey(e => e.BookId);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Reservations)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

