using CatalogServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogServer.Repositories
{
    public interface IBookRepository
    {
        Task<Book> GetBookById(int id);
        Task<IEnumerable<Book>> GetBooksByTopic(string topic);
        void UpdateBook(int id);
    }
}
