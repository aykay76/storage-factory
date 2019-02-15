using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helpers.Storage
{
    public class LocalFileSystem : IStorage
    {
        public string Path { get; set; }

        public LocalFileSystem(string path)
        {
            Path = path;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<T> LoadEntity<T>(string type, string name)
        {
            if (!File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}{type}{System.IO.Path.DirectorySeparatorChar}{name}"))
            {
                throw new FileNotFoundException();
            }

            string json = File.ReadAllText($"{Path}{System.IO.Path.DirectorySeparatorChar}{name}");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task SaveEntity(string type, string name, object value)
        {
            try
            {
                string file = $"{Path}{System.IO.Path.DirectorySeparatorChar}{type}{System.IO.Path.DirectorySeparatorChar}{name}";
                string json = JsonConvert.SerializeObject(value);

                string directory = System.IO.Path.GetDirectoryName(file);
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(file, json);

            }
            catch (Exception)
            {

            }
        }

        public async Task DeleteEntity(string type, string name)
        {
            File.Delete($"{Path}{System.IO.Path.DirectorySeparatorChar}{type}{System.IO.Path.DirectorySeparatorChar}{name}");
        }

        public async Task CreateIfNotExist()
        {
            Directory.CreateDirectory(Path);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
