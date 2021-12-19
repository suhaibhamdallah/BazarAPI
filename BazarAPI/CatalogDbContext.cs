using CatalogServer.Enums;
using CatalogServer.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogServer
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public CatalogDbContext()
        {

        }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Bazar.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasData(
                    new Book { Id = 1, Title = "How to get a good grade in DOS in 40 minutes a day", NumberOfItemsInStock = 7, Price = 40.50m, Topic = BooksTopics.DistributedSystems },
                    new Book { Id = 2, Title = "RPCs for Noobs", NumberOfItemsInStock = 6, Price = 30.50m, Topic = BooksTopics.DistributedSystems },
                    new Book { Id = 3, Title = "Xen and the Art of Surviving Undergraduate School", NumberOfItemsInStock = 8, Price = 50.00m, Topic = BooksTopics.UndergraduateSchool },
                    new Book { Id = 4, Title = "Cooking for the Impatient Undergrad", NumberOfItemsInStock = 11, Price = 10.99m, Topic = BooksTopics.UndergraduateSchool },
                    new Book { Id = 5, Title = "How to finish Project 3 on time", NumberOfItemsInStock = 11, Price = 10.99m, Topic = BooksTopics.New },
                    new Book { Id = 6, Title = "Why theory classes are so hard", NumberOfItemsInStock = 11, Price = 10.99m, Topic = BooksTopics.New },
                    new Book { Id = 7, Title = "Spring in the Pioneer Valley", NumberOfItemsInStock = 11, Price = 10.99m, Topic = BooksTopics.New }
                );
        }
    }
}