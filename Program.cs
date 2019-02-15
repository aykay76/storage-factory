using System;

namespace storage_factory
{
    class Program
    {
        static void Main(string[] args)
        {
            DataType value = new DataType();
            IStorage storage = StorageFactory.ForLocalFileSystem(System.IO.Path.GetTempPath()).GetAwaiter().GetResult();
            storage.SaveEntity("type", "name", value);
        }
    }
}
