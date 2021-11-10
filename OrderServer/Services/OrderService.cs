using OrderServer.Models;
using OrderServer.Repositories;
using System.Threading.Tasks;

namespace OrderServer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> AddOrder(Order order)
        {
            return await _orderRepository.AddNewOrder(order);
        }
    }
}
