using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static readonly List<string> _catalogServersIps = new List<string>();
        private static readonly List<string> _orderServersIps = new List<string>();
        private static int _roundRoubinCounterForCatalogServer = 1;
        private static int _roundRoubinCounterForOrderServer = 1;
        private static readonly MemoryCache cache = new MemoryCache("bazarCache1");

        public static void Main()
        {
            var connection = new HubConnectionBuilder()
           .WithUrl("http://192.168.56.105/notify")
           .Build();

            connection.On("ReceiveMessage", (string msg) =>
            {
                cache.Remove(msg);
            });

            connection.StartAsync().Wait();


            var connection2 = new HubConnectionBuilder()
           .WithUrl("http://192.168.56.107/notify")
           .Build();

            connection2.On("ReceiveMessage", (string msg) =>
            {
                cache.Remove(msg);
            });

            connection2.StartAsync().Wait();


            _catalogServersIps.Add("http://192.168.56.105");
            _catalogServersIps.Add("http://192.168.56.107");

            _orderServersIps.Add("http://192.168.56.104");
            _orderServersIps.Add("http://192.168.56.108");

            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            while (true)
            {
                Console.WriteLine("Welcome to Bazar");
                Console.WriteLine("Choose operation do you want from belows:");
                Console.WriteLine("1. Book query by Id");
                Console.WriteLine("2. Book query by topic");
                Console.WriteLine("3. Purchase a book");

                var operation = int.Parse(Console.ReadLine());

                switch (operation)
                {
                    case 1:
                        Console.WriteLine("Please enter book id:");
                        int id = int.Parse(Console.ReadLine());
                        await GetBookById(id);
                        break;

                    case 2:
                        Console.WriteLine("Please enter book topic:");
                        string topic = Console.ReadLine();
                        await GetBooksByTopic(topic);
                        break;

                    case 3:
                        Console.WriteLine("Please enter book id you want to purchase:");
                        int id2 = int.Parse(Console.ReadLine());
                        await PurchaseBook(id2);
                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }

                Console.WriteLine("****************************");
            }
        }

        public static async Task GetBookById(int id)
        {
            if (cache.Contains(id.ToString()))
            {
                var cacheResult = cache.GetCacheItem(id.ToString());
                var bookFromCache = (Book)cacheResult.Value;

                Console.WriteLine($"Book Id: {bookFromCache.Id}");
                Console.WriteLine($"Book Title: {bookFromCache.Title}");
                Console.WriteLine($"Book Topic: {bookFromCache.Topic}");
                Console.WriteLine($"Book Price: {bookFromCache.Price}");
                Console.WriteLine($"Book Number of items in stock: {bookFromCache.NumberOfItemsInStock}");
                Console.WriteLine("From cache");
                return;
            }

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
                var response = await httpClient.GetAsync($"{catalogServerIp}/info/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Invalid book id");
                }
                else
                {
                    var book = JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());

                    var isAdded = cache.Add(id.ToString(), book, DateTimeOffset.Now.AddMinutes(10));

                    Console.WriteLine($"Book Id: {book.Id}");
                    Console.WriteLine($"Book Title: {book.Title}");
                    Console.WriteLine($"Book Topic: {book.Topic}");
                    Console.WriteLine($"Book Price: {book.Price}");
                    Console.WriteLine($"Book Number of items in stock: {book.NumberOfItemsInStock}");
                }
            }
        }

        public static async Task GetBooksByTopic(string topic)
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
                var response = await httpClient.GetAsync($"{catalogServerIp}/search/{topic}");

                var books = JsonConvert.DeserializeObject<List<Book>>(await response.Content.ReadAsStringAsync())
                    .Where(book => book.NumberOfItemsInStock > 0).ToList();

                books.ForEach(book =>
                {
                    Console.WriteLine($"Book Id: {book.Id}");
                    Console.WriteLine($"Book Title: {book.Title}");
                    Console.WriteLine($"Book Topic: {book.Topic}");
                    Console.WriteLine($"Book Price: {book.Price}");
                    Console.WriteLine($"Book Number of items in stock: {book.NumberOfItemsInStock}");
                    Console.WriteLine("######");
                });
            }
        }

        public static async Task PurchaseBook(int id)
        {
            _roundRoubinCounterForOrderServer++;

            if (_roundRoubinCounterForOrderServer == int.MaxValue) _roundRoubinCounterForOrderServer = 1;

            string orderServerIp = string.Empty;

            if (_roundRoubinCounterForOrderServer % 2 == 0)
            {
                orderServerIp = _orderServersIps[0];
            }
            else
            {
                orderServerIp = _orderServersIps[1];
            }

            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{orderServerIp}/purchase/{id}"))
                {
                    var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid id or quantity");
                    }
                    else
                    {
                        var orderInfo = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
                        Console.WriteLine($"Order Id: {orderInfo.Id}");
                        Console.WriteLine($"Book Id: {orderInfo.BookId}");
                        Console.WriteLine($"Quantity: {orderInfo.Quantity}");
                    }
                }
            }
        }
    }
}