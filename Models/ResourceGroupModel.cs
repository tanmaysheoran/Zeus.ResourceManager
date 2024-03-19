using Azure.Core;
using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.ResourceManager.Models
{
    public class ResourceGroupModel
    {
        public SubscriptionResource SubscriptionResource {set; get; }
        public string ResourceGroupName { set; get; }
        public AzureLocation ResourceGroupLocation { set; get; }
        public Dictionary<string, string> Tags { set; get; }
    }
}
