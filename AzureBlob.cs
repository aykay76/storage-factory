using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Helpers.Storage
{
    public class AzureBlob : IStorage
    {
        CloudStorageAccount Account { get; set; }
        CloudBlobClient client = null;
        CloudBlobContainer container = null;

        string ConnectionString { get; set; }
        string ContainerName { get; set; }

        public AzureBlob(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
            Account = CloudStorageAccount.Parse(connectionString);
            client = Account.CreateCloudBlobClient();
            container = client.GetContainerReference("AppName");
        }

        public async Task<T> LoadEntity<T>(string type, string name)
        {
            T definition = default(T);

            CloudBlobDirectory directory = container.GetDirectoryReference(type);
            CloudPageBlob blob = directory.GetPageBlobReference(name);
            Stream s = await blob.OpenReadAsync();
            long l = s.Length;
            byte[] bytes = new byte[l];
            s.Read(bytes, 0, (int)l);
            s.Close();
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            definition = JsonConvert.DeserializeObject<T>(json);

            return definition;
        }

        public async Task SaveEntity(string type, string name, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            CloudBlobDirectory directory = container.GetDirectoryReference(type);
            CloudPageBlob blob = directory.GetPageBlobReference(name);
            CloudBlobStream stream = await blob.OpenWriteAsync(json.Length);
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json));
            stream.Close();
        }

        public async Task DeleteEntity(string type, string name)
        {
            CloudBlobDirectory directory = container.GetDirectoryReference(type);
            CloudPageBlob blob = directory.GetPageBlobReference(name);
            await blob.DeleteAsync();
        }

        public async Task CreateIfNotExist()
        {
            await container.CreateIfNotExistsAsync();
        }
    }
}
