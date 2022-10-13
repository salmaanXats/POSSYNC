using System.Threading.Tasks;

namespace DataSyncer.Interfaces
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri);
        Task<T> Post<T>(string uri, object value);
        Task Post(string uri, object value);
    }
}
