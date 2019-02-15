using System;
using System.Threading.Tasks;

namespace Helpers.Storage
{
    public interface IStorage
    {
        Task<T> LoadEntity<T>(string type, string name);

        Task SaveEntity(string type, string name, object value);

        Task DeleteEntity(string type, string name);

        Task CreateIfNotExist();
    }
}
