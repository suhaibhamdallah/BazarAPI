using System;
using System.ComponentModel.DataAnnotations;

namespace OrderServer.Models
{
    public class Order
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
