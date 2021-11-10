using CatalogServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogServer.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksByTopic(string topic);
        Task<Book> GetById(int id);
        void UpdateBook(int id);
    }
}
