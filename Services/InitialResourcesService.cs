using Athena.DatabaseHelper.Services;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.ResourceManager.Helpers;
using Zeus.ResourceManager.Models;

namespace Zeus.ResourceManager.Services
{
    public class InitialResourcesService
    {
        private static bool _resourceGroupCreated;
        private ResourceGroupResource _resourceGroupResource;
        public async Task InitialResourcesAsync(CreateInitialResourcesModel input)
        {
            GremlinService gremlinService = new GremlinService();
            var gremlinClient =  GremlinHelper.GetIdentityGremlinClient(gremlinService);
            try
            {
                ArmClientHelper armClientHelper = new ArmClientHelper();
                string subscriptionId = EnvironmentHelper.GetSubscriptionId();

                ArmClient armClient = armClientHelper.CreateArmClient(subscriptionId);
                SubscriptionResource subscriptionResource = armClientHelper.GetSubscriptionResource(armClient);

                ResourceGroupHelper resourceGroupHelper = new ResourceGroupHelper(subscriptionResource, input.location, input.tenantId);
                ResourceGroupResource resourceGroupResource = resourceGroupHelper.CreateNewResourceGroup(new Azure.Core.AzureLocation(input.location), null);

                _resourceGroupCreated = true;
                _resourceGroupResource = resourceGroupResource;

                StorageAccountModel storageAccountModel = new StorageAccountModel(resourceGroupResource,input.creationTime,input.location);
                storageAccountModel.AccountType = input.storageAccount.accountType;
                storageAccountModel.ContainerConfigs = input.storageAccount.containerConfigs;
                string storageAccountConnStr = await StorageAccountService.InitializeStorageAccountAsync(storageAccountModel);

                CosmosAccountModel cosmosAccountModel = new CosmosAccountModel(input.location,false,input.creationTime);
                cosmosAccountModel.ResourceGroup = resourceGroupResource;
                cosmosAccountModel.isServerless = input.cosmosAccount.isServerless;
                cosmosAccountModel.Containers = input.cosmosAccount.containers;
                string cosmosAccountConnStr = CosmosAccountService.InitializeCosmosResources(cosmosAccountModel);

                CosmosAccountModel gremlinAccountModel = new CosmosAccountModel(input.location, true, input.creationTime);
                gremlinAccountModel.ResourceGroup = resourceGroupResource;
                gremlinAccountModel.isServerless = input.gremlinAccount.isServerless;
                gremlinAccountModel.Containers = input.gremlinAccount.containers;
                string gremlinAccountConnStr = CosmosAccountService.InitializeCosmosResources(gremlinAccountModel);

                await GremlinHelper.AddConnectionStringToIdentity(input.tenantId, "st", "cosmosAccountConnStr", "gremlinAccountConnStr",gremlinClient,gremlinService);
            }
            catch(Exception ex)
            {
                if (_resourceGroupCreated)
                {
                    await ResourceGroupHelper.DeleteReourceGroup(_resourceGroupResource);
                    await GremlinHelper.DeleteTenantRecordOnFail(input.tenantId, gremlinClient, gremlinService);
                }
            }

        }
    }
}
