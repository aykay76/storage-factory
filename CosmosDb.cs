using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Helpers.Storage
{
    public class CosmosDb : IStorage
    {
        public Uri ServiceEndpoint { get; set; }
        public SecureString AccessKey { get; set; }
        public string Collection { get; set; }
        public DocumentClient Client { get; set; }

        public CosmosDb(Uri uri, SecureString key)
        {
            ServiceEndpoint = uri;
            AccessKey = key;
            Client = new DocumentClient(uri, key);
        }

        public async Task<T> LoadEntity<T>(string type, string name)
        {
            ResourceResponse<Document> response = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri("appname", type, name));
            string json = response.Resource.ToString();

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task SaveEntity(string type, string name, object value) 
        {
            await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("appname"), new DocumentCollection { Id = type });

            string json = JsonConvert.SerializeObject(value);

            try
            {
                await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri("appname", type, name));
                await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("appname", type, name), value);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("appname", Collection), value);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeleteEntity(string type, string name)
        {
            await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("appname", type, name));
        }

        public async Task CreateIfNotExist()
        {
            await Client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database { Id = "appname" });
        }
    }
}
