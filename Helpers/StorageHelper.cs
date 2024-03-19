using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using System.ComponentModel.DataAnnotations;
using Zeus.ResourceManager.Models;

namespace Zeus.ResourceManager.Helpers
{
    public class StorageHelper
    {

        private static async Task<StorageAccountResource> CreateStorageAccountAsync(ResourceGroupResource resourceGroupResource, string accountType, string accountName, string location)
        {
            StorageSku sku = new StorageSku(accountType);
            StorageKind kind = StorageKind.StorageV2;

            StorageAccountCreateOrUpdateContent parameters = new StorageAccountCreateOrUpdateContent(sku, kind, new Azure.Core.AzureLocation(location));
            StorageAccountCollection accountCollection = resourceGroupResource.GetStorageAccounts();

            ArmOperation<StorageAccountResource> accountCreateOperation = await accountCollection.CreateOrUpdateAsync(WaitUntil.Completed, accountName, parameters);
            StorageAccountResource storageAccount = accountCreateOperation.Value;

            return storageAccount;
        }
        private static async Task CreateContainerInStorageAccountAsync(StorageAccountResource storageAccountResource, string containerName, string storagePublicAccessType = null)
        {
            BlobServiceResource blobService = storageAccountResource.GetBlobService();
            BlobContainerCollection blobContainerCollection = blobService.GetBlobContainers();
            BlobContainerData blobContainerData = new BlobContainerData();
            
            switch (storagePublicAccessType.ToLower())
            {
                case "none":
                    blobContainerData.PublicAccess = StoragePublicAccessType.None;
                    break;
                case "container":
                    blobContainerData.PublicAccess = StoragePublicAccessType.Container;
                    break;
                case "blob":
                    blobContainerData.PublicAccess = StoragePublicAccessType.Blob;
                    break;
                default:
                    blobContainerData.PublicAccess = StoragePublicAccessType.Container;
                    break;
            }

            await blobContainerCollection.CreateOrUpdateAsync(WaitUntil.Completed, containerName, blobContainerData);
        }
        private static async Task CreateMultipleContainersInStorageAccountAsync(StorageAccountResource storageAccountResource, List<StorageContainerConfig> containers)
        {
            List<Task> tasks = new List<Task>();
            foreach (var container in containers)
            {
                 tasks.Add(CreateContainerInStorageAccountAsync(storageAccountResource, container.ContainerName, container.StoragePublicAccessType));
            }

            await Task.WhenAll(tasks);
        }
        public static async Task<StorageAccountResource> CreateNewStorageAccountAsync(ResourceGroupResource resourceGroupResource, string accountType, string accountName,string location, List<StorageContainerConfig> containerConfigs)
        {
            StorageAccountResource accountResource =  await CreateStorageAccountAsync(resourceGroupResource,accountType,accountName,location);
            if(containerConfigs != null && containerConfigs.Count > 0)
            {
                if(containerConfigs.Count == 1)
                    await CreateContainerInStorageAccountAsync(accountResource, containerConfigs.First().ContainerName, containerConfigs.First().StoragePublicAccessType);
                else
                    await CreateMultipleContainersInStorageAccountAsync(accountResource, containerConfigs);
            }

            return accountResource;
        }
        protected StorageAccountResource GetStorageAccountResource(ResourceGroupResource resourceGroupResource, string accountName)
        {
            var allStorageAccounts = resourceGroupResource.GetStorageAccounts();
            var accountResource = allStorageAccounts.Get(accountName);
            return accountResource;
        }

        public static string GetAccountConnectionString(StorageAccountResource accountResource, string accountName)
        {
            var keys = accountResource.GetKeys();
            var key = keys.First().Value;
            var connStr = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={key};EndpointSuffix=core.windows.net";
            return connStr;
        }
    }
}
