using CatalogServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogServer.Repositories
{
    public class BookRepository : IBookRepository
    {
        protected CatalogDbContext dbContext;

        public BookRepository(CatalogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Book> GetBookById(int id)
        {
            return await dbContext.Books
                .FirstOrDefaultAsync(book => book.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByTopic(string topic)
        {
            return await dbContext.Books
                .Where(book => book.Topic == topic)
                .ToListAsync();
        }

        public void UpdateBook(int id)
        {
            var bookFromDb = GetBookById(id).Result;
            bookFromDb.NumberOfItemsInStock -= 1;
            dbContext.Books.Update(bookFromDb);
            dbContext.SaveChanges();
        }
    }
}
