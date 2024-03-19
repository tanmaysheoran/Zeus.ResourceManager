using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.ResourceManager.Models
{

    public class CreateInitialResourcesModel
    {
        public string location { get; set; }
        public string tenantId { get; set; }
        public long creationTime { get; set; }
        public StorageAccount storageAccount { get; set; }
        public CosmosAccount cosmosAccount { get; set; }
        public CosmosAccount gremlinAccount { get; set; }
    }

    public class StorageAccount
    {
        public string accountType { get; set; }
        public List<StorageContainerConfig> containerConfigs { get; set; }
    }
    public class CosmosAccount
    {
        public bool isServerless { get; set; }
        public bool isGremlinAccount { get; set; }
        public List<string> containers { get; set; }
    }
}
