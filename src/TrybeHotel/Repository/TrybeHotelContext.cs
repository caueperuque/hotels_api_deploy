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
        optionsBuilder.UseSqlServer(@"Servermysql://clsj27dhxudvupsd:vrvrbqfz6smd92e2@qao3ibsa7hhgecbv.cbetxkdyhwsb.us-east-1.rds.amazonaws.com:3306/u58myi75xtv2xmfs;Database=u58myi75xtv2xmfs;User=clsj27dhxudvupsd;Password=vrvrbqfz6smd92e2;TrustServerCertificate=True");
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