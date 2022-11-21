using Redis.OM.Modeling;

namespace AutoCompleteDemo.Models
{
    [Document(StorageType = StorageType.Json, IndexName = "airport-idx")]
    public partial class Airport
    {
        [RedisIdField]
        public string Id { get; set; }
        [Indexed]
        public string Name { get; set; }
        [Indexed]
        public string Code { get; set; }
        [Indexed]
        public string State { get; set; }
    }
}
