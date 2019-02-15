using System;
using System.Security;
using System.Threading.Tasks;

namespace Helpers.Storage
{
    public static class StorageFactory
    {
        public static async Task<IStorage> ForLocalFileSystem(string path)
        {
            LocalFileSystem lfs = new LocalFileSystem(path);
            await lfs.CreateIfNotExist();
            return lfs;
        }

        public static async Task<IStorage> ForAzureBlob(string connectionString, string containerName)
        {
            AzureBlob ab = new AzureBlob(connectionString, containerName);
            await ab.CreateIfNotExist();
            return ab;
        }

        public static async Task<IStorage> ForAzureFile(string account, string key)
        {
            AzureFile af = new AzureFile(account, key);
            await af.CreateIfNotExist();
            return af;
        }

        public static async Task<IStorage> ForCosmosDb(Uri uri, SecureString key)
        {
            CosmosDb db = new CosmosDb(uri, key);
            await db.CreateIfNotExist();
            return db;
        }
    }
}
