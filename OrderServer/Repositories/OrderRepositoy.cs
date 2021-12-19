using OrderServer.Models;
using System.Threading.Tasks;

namespace OrderServer.Repositories
{
    public class OrderRepositoy : IOrderRepository
    {
        protected OrderDbContext dbContext;

        public OrderRepositoy(OrderDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Order> AddNewOrder(Order order)
        {
            var orderCreated = await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return orderCreated.Entity;
        }
    }
}
