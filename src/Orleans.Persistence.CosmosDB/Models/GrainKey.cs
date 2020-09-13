namespace Orleans.Persistence.CosmosDB.Models
{
    public class GrainKey<TRootKey>
    {
        public GrainKey(TRootKey key, string keyExtension = null)
        {
            this.Key = key;
            this.KeyExtension = keyExtension;
        }

        public TRootKey Key { get; private set; }
        public string KeyExtension { get; private set; }
    }
}
