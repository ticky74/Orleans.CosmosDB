using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Orleans.Persistence.CosmosDB.Models
{
    internal class GrainStateEntity
    {
        private const string ETAG_FIELD = "_etag";
        private const string ID_FIELD = "id";

        [JsonProperty(ETAG_FIELD)]
        [DataMember(Name = ETAG_FIELD)]
        public string ETag { get; set; }

        [JsonProperty(nameof(GrainType))]
        public string GrainType { get; set; }

        [JsonProperty(ID_FIELD)]
        [DataMember(Name = ID_FIELD)]
        public string Id { get; set; }

        [JsonProperty(nameof(PartitionKey))]
        public string PartitionKey { get; set; }

        [JsonProperty(nameof(State))]
        public object State { get; set; }
    }
}
