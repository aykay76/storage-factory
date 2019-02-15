using System;
using Newtonsoft.Json;

namespace storage_factory
{
    public class DataType
    {
        [JsonProperty("propertyName")]
        public string PropertyName { get; set; }
    }
}