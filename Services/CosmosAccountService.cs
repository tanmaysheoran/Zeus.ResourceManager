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
    public class CosmosAccountService
    {
        public static string InitializeCosmosResources(CosmosAccountModel accountModel)
        {
            CosmosAccountHelper cosmosAccountHelper = new CosmosAccountHelper();
            var accountResource = cosmosAccountHelper.CreateCosmosDBAccount(accountModel.ResourceGroup,accountModel.isServerless,accountModel.isGremlinAccount,accountModel.ThroughputLimit,accountModel.AccountName,accountModel.Location,accountModel.GeoReplicationEnabledLocations);
            var database = cosmosAccountHelper.CreateDatbaseAsync(accountResource,accountModel.DatabaseName).Result;
            cosmosAccountHelper.CreateMultipleContainers(database,accountModel.Containers,accountModel.isGremlinAccount);
            string connStr = cosmosAccountHelper.GetAccountConnectionString(accountResource,accountModel.DatabaseName,accountModel.AccountName,accountModel.isGremlinAccount);
            return connStr;
        }
    }
}
