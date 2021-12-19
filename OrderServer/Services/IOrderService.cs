using OrderServer.Models;
using System.Threading.Tasks;

namespace OrderServer.Services
{
    public interface IOrderService
    {
        Task<Order> AddOrder(Order order);
    }
}
