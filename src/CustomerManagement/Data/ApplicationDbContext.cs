using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=localhost;Database=CustomerManagementDB;User=SA;Password=Password123;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);

                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("FirstName");

                entity.Property(c => c.LastName)
                    .HasMaxLength(40)
                    .HasColumnName("LastName");

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Email");

                entity.Property(c => c.DateOfBirth)
                    .IsRequired()
                    .HasColumnName("DateOfBirth");

                entity.HasIndex(c => c.Email)
                    .IsUnique();

                entity.HasMany(c => c.Addresses)
                    .WithOne()
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.AddressId);

                entity.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("ZipCode");

                entity.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Street");

                entity.Property(a => a.Number)
                .IsRequired()
                .HasColumnName("Number");

                entity.Property(a => a.Neighborhood)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Neighborhood");

                entity.Property(a => a.AddressComplement)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("AddressComplement");

                entity.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("City");

                entity.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("State");

                entity.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Country");

                entity.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId);
                //.OnDelete(DeleteBehavior.Restrict);
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}