namespace CatalogServer.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public int NumberOfItemsInStock { get; set; }
        public decimal Price { get; set; }
    }
}
