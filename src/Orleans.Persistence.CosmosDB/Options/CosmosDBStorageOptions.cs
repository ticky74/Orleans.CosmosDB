using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Orleans.Runtime;
using System.Collections.Generic;

namespace Orleans.Persistence.CosmosDB
{
    public class CosmosDBStorageOptions
    {
        public const int DEFAULT_INIT_STAGE = ServiceLifecycleStage.ApplicationServices;
        internal const string ORLEANS_STORAGE_COLLECTION = "OrleansStorage";
        private const string ORLEANS_DB = "Orleans";
        private const int ORLEANS_STORAGE_COLLECTION_THROUGHPUT = 400;

        public string AccountEndpoint { get; set; }

        [Redact]
        public string AccountKey { get; set; }

        public bool CanCreateResources { get; set; }
        public CosmosClient Client { get; set; }
        public string Collection { get; set; } = ORLEANS_STORAGE_COLLECTION;

        /// <summary>
        /// RU units for collection, can be set to 0 if throughput is specified on database level. See https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput
        /// </summary>
        public int CollectionThroughput { get; set; } = ORLEANS_STORAGE_COLLECTION_THROUGHPUT;

        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.Direct;

        public CosmosSerializer CustomSerializer { get; set; }

        /// <summary>
        /// Database configured throughput, if set to 0 it will not be configured and collection throughput must be set. See https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput
        /// </summary>
        public int DatabaseThroughput { get; set; } = ORLEANS_STORAGE_COLLECTION_THROUGHPUT;

        public string DB { get; set; } = ORLEANS_DB;
        public bool DeleteStateOnClear { get; set; }

        /// <summary>
        /// Delete the database on initialization.  Useful for testing scenarios.
        /// </summary>
        public bool DropDatabaseOnInit { get; set; }

        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// Stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = DEFAULT_INIT_STAGE;

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// List of JSON path strings.
        /// Each entry on this list represents a property in the State Object that will be included in the document index.
        /// The default is to not add any property in the State object.
        /// </summary>
        public List<string> StateFieldsToIndex { get; set; } = new List<string>();

        [JsonConverter(typeof(StringEnumConverter))]
        public TypeNameHandling TypeNameHandling { get; set; } = TypeNameHandling.All;

        public bool UseFullAssemblyNames { get; set; } = true;

        // TODO: Consistency level for emulator (defaults to Session; https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
        internal ConsistencyLevel? GetConsistencyLevel() => !string.IsNullOrWhiteSpace(this.AccountEndpoint) && this.AccountEndpoint.Contains("localhost") ? (ConsistencyLevel?)ConsistencyLevel.Session : null;
    }

    /// <summary>
    /// Configuration validator for CosmosDBStorageOptions
    /// </summary>
    public class CosmosDBStorageOptionsValidator : IConfigurationValidator
    {
        private readonly string name;
        private readonly CosmosDBStorageOptions options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The option to be validated.</param>
        /// <param name="name">The option name to be validated.</param>
        public CosmosDBStorageOptionsValidator(CosmosDBStorageOptions options, string name)
        {
            this.options = options;
            this.name = name;
        }

        public void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(this.options.DB))
                throw new OrleansConfigurationException(
                    $"Configuration for CosmosDBStorage {this.name} is invalid. {nameof(this.options.DB)} is not valid.");

            if (string.IsNullOrWhiteSpace(this.options.Collection))
                throw new OrleansConfigurationException(
                    $"Configuration for CosmosDBStorage {this.name} is invalid. {nameof(this.options.Collection)} is not valid.");

            if (this.options.CollectionThroughput < 400 && this.options.DatabaseThroughput < 400)
                throw new OrleansConfigurationException(
                    $"Configuration for CosmosDBStorage {this.name} is invalid. Either {nameof(this.options.DatabaseThroughput)} or {nameof(this.options.CollectionThroughput)} must exceed 400.");
        }
    }
}
