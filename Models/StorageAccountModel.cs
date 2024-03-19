using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.ResourceManager.Helpers;

namespace Zeus.ResourceManager.Models
{
    public class StorageAccountModel
    {
        public StorageAccountModel(ResourceGroupResource resourceGroup, long creationTime, string location)
        {
            ResourceGroup = resourceGroup;
            //var shorten = Convert.ToBase64String(Guid.Parse(tenantId).ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 10).ToLower();
            AccountName = creationTime.ToString();
            Location = location;
        }

        public ResourceGroupResource ResourceGroup { get; set; } = null;
        public string Location { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; } = "Standard_LRS";
        public List<StorageContainerConfig> ContainerConfigs { get; set; }
        public string TenantId { get; set; }
    }
    public class StorageContainerConfig
    {
        public string ContainerName { get; set; }
        public string StoragePublicAccessType { get; set; } = "default";
    }
}
