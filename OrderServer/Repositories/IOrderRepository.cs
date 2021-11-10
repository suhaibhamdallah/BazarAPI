using OrderServer.Models;
using System.Threading.Tasks;

namespace OrderServer.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddNewOrder(Order order);
    }
}
