using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderServer.Models;
using OrderServer.Services;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderServer.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("purchase/{id}")]
        public async Task<ActionResult<Order>> PurchaseOrder([FromRoute]int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpClientResponse = await httpClient.GetAsync($"http://192.168.56.105/info/{id}"))
                {
                    if (!httpClientResponse.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }
                }
            }

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Patch, "http://192.168.56.105/update"))
                {
                    var json = JsonConvert.SerializeObject(new { Id = id });
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;

                        using (var response = await client
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                            .ConfigureAwait(false))
                        {
                            response.EnsureSuccessStatusCode();
                        }
                    }
                }
            }

            var orderToPurchase = new Order()
            {
                BookId = id,
                Quantity = 1
            };

            var orderCreated = await _orderService.AddOrder(orderToPurchase);
            return Ok(orderCreated);
        }
    }
}
