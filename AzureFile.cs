using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;

namespace Helpers.Storage
{
    public class AzureFile : IStorage
    {
        CloudStorageAccount Account { get; set; }
        string AccountName { get; set; }
        CloudFileClient client = null;
        CloudFileShare shareReference = null;
        CloudFileDirectory rootDirectory = null;

        public AzureFile(string accountName, string key)
        {
            Account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, key), "core.windows.net", true);
            AccountName = accountName;
        }

        public async Task<T> LoadEntity<T>(string type, string name)
        {
            CloudFileDirectory directory = rootDirectory.GetDirectoryReference(type);
            if (await directory.ExistsAsync() == false)
            {
                throw new FileNotFoundException();
            }

            CloudFile file = directory.GetFileReference(name);
            if (await file.ExistsAsync() == false)
            {
                throw new FileNotFoundException();
            }

            T definition = default(T);
            try
            {
                Stream s = await file.OpenReadAsync();
                long l = s.Length;
                byte[] bytes = new byte[l];
                s.Read(bytes, 0, (int)l);
                s.Close();
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                definition = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {

            }

            return definition;
        }

        public async Task SaveEntity(string type, string name, object value)
        {
            CloudFileDirectory directory = rootDirectory.GetDirectoryReference(type);
            await directory.CreateIfNotExistsAsync();

            string json = JsonConvert.SerializeObject(value);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

            CloudFileStream stream = await directory.GetFileReference(name).OpenWriteAsync(bytes.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            stream.Close();
        }

        public async Task DeleteEntity(string type, string name)
        {
            CloudFileDirectory directory = rootDirectory.GetDirectoryReference(type);
            CloudFile file = directory.GetFileReference(name);
            await file.DeleteAsync();
        }

        public async Task CreateIfNotExist()
        {
            client = Account.CreateCloudFileClient();
            shareReference = client.GetShareReference("appname");
            await shareReference.CreateIfNotExistsAsync();
            rootDirectory = shareReference.GetRootDirectoryReference();
        }
    }
}
