using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using System.Threading;
using Zeus.ResourceManager.Models;

namespace Zeus.ResourceManager.Helpers
{
    public class ResourceGroupHelper: ResourceGroupModel
    {
        public ResourceGroupHelper(SubscriptionResource subscriptionResource,string location ,string tenantId)
        {
            SubscriptionResource = subscriptionResource;
            ResourceGroupName = $"rg-{EnvironmentHelper.GetCurrentEnv()}-{tenantId}";
            ResourceGroupLocation = string.IsNullOrEmpty(location) ? "centralindia" : new AzureLocation(location);
        }
        private ResourceGroupResource CreateResourceGroup(SubscriptionResource subscriptionResource, string resourceGroupName, AzureLocation resourceGroupLocation)
        {
            var allResourceGroups = subscriptionResource.GetResourceGroups();
            if (allResourceGroups.Exists(resourceGroupName))
            {
                throw new InvalidOperationException($"Resource Group with name: {resourceGroupName} already exists.");
            }

            var resourceGroup = allResourceGroups.CreateOrUpdate(WaitUntil.Completed, resourceGroupName, new ResourceGroupData(resourceGroupLocation)).Value;
            return resourceGroup;
        }
        private void AddTagsToResourceGroup(ResourceGroupResource resourceGroup, Dictionary<string,string> tags, CancellationToken cancellationToken = default(CancellationToken))
        {
            Response<TagResource> response = resourceGroup.GetTagResource().Get(cancellationToken);
            var payload = response.Value.Data.TagValues;
            foreach(var tag in tags)
            {
                payload[tag.Key] = tag.Value;
            }
            resourceGroup.GetTagResource().CreateOrUpdateAsync(WaitUntil.Started, response.Value.Data, cancellationToken);
        }
        public ResourceGroupResource CreateNewResourceGroup(AzureLocation resourceGroupLocation, Dictionary<string, string> tags)
        {
            var rgResource = CreateResourceGroup(SubscriptionResource, ResourceGroupName, resourceGroupLocation);
            if(tags != null)
            {
                AddTagsToResourceGroup(rgResource, tags);
            }

            return rgResource;
        }
        protected ResourceGroupResource GetResourceGroup()
        {
            var allResourceGroups = SubscriptionResource.GetResourceGroups();
            var resourceGroup = allResourceGroups.Get(ResourceGroupName);
            return resourceGroup;
        }

        public static async Task DeleteReourceGroup(ResourceGroupResource resourceGroup)
        {
            await resourceGroup.DeleteAsync(WaitUntil.Completed);
        }
    } 
}
