using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderServer.Models;
using OrderServer.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderServer.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private int _roundRoubinCounterForCatalogServer = 1;
        private List<string> _catalogServersIps = new List<string>();

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;

            _catalogServersIps.Add("http://192.168.56.105");
            _catalogServersIps.Add("http://192.168.56.107");
        }

        [HttpPost]
        [Route("purchase/{id}")]
        public async Task<ActionResult<Order>> PurchaseOrder([FromRoute]int id)
        {
            _roundRoubinCounterForCatalogServer++;

            if (_roundRoubinCounterForCatalogServer == int.MaxValue) _roundRoubinCounterForCatalogServer = 1;

            string catalogServerIp = string.Empty;

            if (_roundRoubinCounterForCatalogServer % 2 == 0)
            {
                catalogServerIp = _catalogServersIps[0];
            }
            else
            {
                catalogServerIp = _catalogServersIps[1];
            }

            using (var httpClient = new HttpClient())
            {
                using (var httpClientResponse = await httpClient.GetAsync($"{catalogServerIp}/info/{id}"))
                {
                    if (!httpClientResponse.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }
                }
            }

            _catalogServersIps.ForEach(async server =>
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Patch, $"{server}/update"))
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
            });

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