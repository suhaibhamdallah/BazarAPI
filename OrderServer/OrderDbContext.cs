using Microsoft.EntityFrameworkCore;
using OrderServer.Models;

namespace OrderServer
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext()
        {

        }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Order.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(eb =>
            {
                eb.HasKey(c => c.Id);
            });
        }
    }
}
