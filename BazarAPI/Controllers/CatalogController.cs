using CatalogServer.Models;
using CatalogServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogServer.Controllers
{
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IBookService _bookService;

        public CatalogController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet()]
        [Route("search/{topic}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByTopic([FromRoute] string topic)
        {
            var booksFromService = await _bookService.GetBooksByTopic(topic);

            return Ok(booksFromService);
        }

        [HttpGet()]
        [Route("info/{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var bookFromService = await _bookService.GetById(id);

            if (bookFromService is null || bookFromService.NumberOfItemsInStock == 0)
                return NotFound();

            return Ok(bookFromService);
        }

        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> UpdateBook([FromBody] BookForPatch bookForPatch)
        {
            _bookService.UpdateBook(bookForPatch.Id);
            return NoContent();
        }
    }
}
