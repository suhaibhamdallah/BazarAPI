using CatalogServer.Models;
using CatalogServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogServer.Controllers
{
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IBookService _bookService;
        private IHubContext<InformHub> _informHub;

        public CatalogController(IBookService bookService, IHubContext<InformHub> hubContext)
        {
            _bookService = bookService;

            _informHub = hubContext;
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
            await _informHub.Clients.All.SendAsync("ReceiveMessage", bookForPatch.Id.ToString());
            _bookService.UpdateBook(bookForPatch.Id);
            return NoContent();
        }
    }
}