using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
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
                    .HasDefaultValue(DateOnly.FromDateTime(DateTime.Now))
                    .HasColumnName("DateOfBirth");

                entity.HasIndex(c => c.Email)
                    .IsUnique();

                entity.HasMany(c => c.Addresses)
                    .WithOne()
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Orders)
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
            });

            modelBuilder.Entity<Order>(entity =>
                {
                    entity.HasKey(o => o.OrderId);

                    entity.Property(o => o.Number)
                    .HasColumnName("Number")
                    .IsRequired();

                    entity.Property(o => o.Date)
                    .IsRequired()
                    .HasDefaultValue(DateTime.Now) //DateOnly.FromDateTime(DateTime.Now)
                    .HasColumnName("Date");

                    entity.HasOne(c => c.Customer)
                    .WithMany(o => o.Orders)
                    .HasForeignKey(c => c.CustomerId);

                    entity.HasMany(o => o.Itens)
                    .WithOne()
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.Property(o => o.TotalOrderValue)
                    .HasColumnName("TotalOrderValue")
                    .IsRequired();
                }
            );

            modelBuilder.Entity<Item>(entity =>
                {
                    entity.HasKey(i => i.ItemId);

                    entity.Property(i => i.QuantityOfItens)
                    .HasColumnName("QuantityOfItens")
                    .IsRequired();

                    entity.Property(i => i.UnitValue)
                    .HasColumnName("UnitValue")
                    .IsRequired();

                    entity.Property(i => i.TotalValue)
                    .HasColumnName("TotalValue")
                    .IsRequired();

                    entity.HasOne(o => o.Order)
                    .WithMany(i => i.Itens)
                    .HasForeignKey("OrderId");

                    entity.HasOne(p => p.Product)
                    .WithOne(p => p.Item)
                    .HasForeignKey<Product>(p => p.ItemId);
                }
            );

            modelBuilder.Entity<Product>(entity =>
                {
                    entity.HasKey(p => p.Id);

                    entity.Property<string>("_code")
                    .HasMaxLength(40)
                    .HasColumnName("Code")
                    .IsRequired();

                    entity.HasIndex("_code")
                    .IsUnique();

                    entity.Property<string>("_name")
                    .HasMaxLength(40)
                    .HasColumnName("Name")
                    .IsRequired();

                    entity.HasOne(i => i.Item)
                    .WithOne(p => p.Product)
                    .HasForeignKey<Product>(p => p.ItemId);

                    entity.Ignore(p => p.IsValid);
                }
            );
            
            base.OnModelCreating(modelBuilder);
        }
    }
}