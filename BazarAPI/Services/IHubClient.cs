using System.Threading.Tasks;

namespace CatalogServer.Services
{
    public interface IHubClient
    {
        Task SendMessage(string message);
    }
}