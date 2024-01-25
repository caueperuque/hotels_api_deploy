using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options) {
    }
    public TrybeHotelContext() { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=HotelsDB;User=SA;Password=SenhaSuperSecreta12!;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>()
                    .HasKey(h => h.HotelId);

        modelBuilder.Entity<Hotel>()
                    .HasOne(h => h.City)
                    .WithMany(c => c.Hotels)
                    .HasForeignKey(h => h.CityId);

        modelBuilder.Entity<City>()
                    .HasMany(c => c.Hotels)
                    .WithOne(h => h.City)
                    .HasForeignKey(h => h.CityId);

        modelBuilder.Entity<Room>()
                    .HasKey(r => r.RoomId);
        modelBuilder.Entity<Room>()
                    .HasOne(r => r.Hotel)
                    .WithMany(h => h.Rooms)
                    .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Room>()
            .HasMany(b => b.Bookings)
            .WithOne(r => r.Room)
            .HasForeignKey(b => b.RoomId);

        modelBuilder.Entity<User>()
                    .HasKey(u => u.UserId);
        modelBuilder.Entity<User>()
                    .HasMany(b => b.Bookings)
                    .WithOne(u => u.User);
        
        modelBuilder.Entity<Booking>()
                    .HasKey(b => b.BookingId);
        modelBuilder.Entity<Booking>()
                    .HasOne(u => u.User)
                    .WithMany(b => b.Bookings);
                    
    }

}