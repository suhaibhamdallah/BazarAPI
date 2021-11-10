using CatalogServer.Models;
using CatalogServer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogServer.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetBooksByTopic(string topic)
        {
            return await _bookRepository.GetBooksByTopic(topic);
        }

        public async Task<Book> GetById(int id)
        {
            return await _bookRepository.GetBookById(id);
        }

        public void UpdateBook(int id)
        {
            _bookRepository.UpdateBook(id);
        }
    }
}
