using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookStock> BookStocks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<BookFeedback> BookFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-one relationship between Book and BookStock
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Stock)
                .WithOne(bs => bs.Book)
                .HasForeignKey<BookStock>(bs => bs.BookId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure one-to-many relationship between Order and OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Orders) // Navigation property in OrderItem
                .WithMany(o => o.OrderItems) // Navigation property in Orders
                .HasForeignKey(oi => oi.OrderID) // Foreign key in OrderItem
                .HasPrincipalKey(o => o.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Orders and OrderPayments
            modelBuilder.Entity<OrderPayment>()
                .HasOne(op => op.Orders) // Navigation property in OrderPayment
                .WithMany(o => o.OrderPayments) // Navigation property in Orders
                .HasForeignKey(op => op.OrderID) // Foreign key in OrderPayment
                .HasPrincipalKey(o => o.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraint on OrderID for Orders
            modelBuilder.Entity<Orders>()
                .HasIndex(o => o.OrderID)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
