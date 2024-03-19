using Azure.Core;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.ResourceManager.Models
{
    public class CosmosAccountModel
    {
        public CosmosAccountModel(string location, bool isGremlin, long creationTime, string databaseName = null)
        {
            Location = new AzureLocation(location);
            //var shorten = Convert.ToBase64String(Guid.Parse(tenantId).ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 10).ToLower();
            AccountName = isGremlin ? $"{creationTime.ToString()}-gremlin" : $"{creationTime.ToString()}-document";
            DatabaseName = databaseName != null ? databaseName : DatabaseName;
            CosmosDBAccountLocation cosmosDBAccountLocation = new CosmosDBAccountLocation();
            cosmosDBAccountLocation.LocationName = Location;
            GeoReplicationEnabledLocations = new[] { cosmosDBAccountLocation };
        }

        public ResourceGroupResource ResourceGroup { set; get; } = null;
        public string AccountName { set; get; }
        public AzureLocation Location { set; get; }
        public IEnumerable<CosmosDBAccountLocation> GeoReplicationEnabledLocations { get; set; }
        public bool isGremlinAccount { get; set; }
        public bool isServerless { get; set; }
        public int ThroughputLimit { get; set; } = 4000;
        public string DatabaseName { set; get; } = "primary";
        public List<string> Containers { set; get; }
    }
}
