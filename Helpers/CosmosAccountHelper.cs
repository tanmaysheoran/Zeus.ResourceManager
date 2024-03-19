using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Cosmos;
using Zeus.ResourceManager.Models;

namespace Zeus.ResourceManager.Helpers
{
    public class CosmosAccountHelper
    {        

        public CosmosDBAccountResource GetExistingCosmosDBAccountResource(ResourceGroupResource resourceGroup, string accountName)
        {
            CosmosDBAccountResource accountResource = resourceGroup.GetCosmosDBAccount(accountName);
            return accountResource;
        }
        public CosmosDBAccountResource CreateCosmosDBAccount(ResourceGroupResource resourceGroup,bool isServerless,bool isGremlinAccount, int throughputLimit,string accountName, string location, IEnumerable<CosmosDBAccountLocation> geoReplicationEnabledLocations)
        {
            CosmosDBAccountCreateOrUpdateContent cosmosDBAccountCreateOrUpdateContent = new CosmosDBAccountCreateOrUpdateContent(new AzureLocation(location), geoReplicationEnabledLocations);
            CosmosDBAccountCollection cosmosDBs = resourceGroup.GetCosmosDBAccounts();

            if (isServerless)
                cosmosDBAccountCreateOrUpdateContent.Capabilities.Add(new CosmosDBAccountCapability() { Name = "EnableServerless" });
            else
                cosmosDBAccountCreateOrUpdateContent.CapacityTotalThroughputLimit = throughputLimit;


            if (isGremlinAccount)
                cosmosDBAccountCreateOrUpdateContent.Capabilities.Add(new CosmosDBAccountCapability() { Name = "EnableGremlin" });


            //future update -- add code for backup policy
            //cosmosDBAccountCreateOrUpdateContent.BackupPolicy.MigrationState.TargetType = "Periodic";

            ArmOperation<CosmosDBAccountResource> armOperation = cosmosDBs.CreateOrUpdate(WaitUntil.Completed, accountName, cosmosDBAccountCreateOrUpdateContent);
            CosmosDBAccountResource accountResource = armOperation.Value;
            return accountResource;
        }
        public CosmosDBSqlDatabaseResource CreateDatabase(CosmosDBAccountResource cosmosDBAccountResource, string databaseName, string location)
        {
            CosmosDBSqlDatabaseCollection cosmosDBSqlDatabaseCollection = cosmosDBAccountResource.GetCosmosDBSqlDatabases();
            CosmosDBSqlDatabaseResourceInfo cosmosDBSqlDatabaseResourceInfo = new CosmosDBSqlDatabaseResourceInfo(databaseName);
            CosmosDBSqlDatabaseCreateOrUpdateContent cosmosDBSqlDatabaseCreateOrUpdateContent = new CosmosDBSqlDatabaseCreateOrUpdateContent(location,cosmosDBSqlDatabaseResourceInfo);

            ArmOperation<CosmosDBSqlDatabaseResource> cosmosDBSqlDatabaseResourceArm = cosmosDBSqlDatabaseCollection.CreateOrUpdate(WaitUntil.Completed, databaseName, cosmosDBSqlDatabaseCreateOrUpdateContent);
            CosmosDBSqlDatabaseResource cosmosDBSqlDatabaseResource = cosmosDBSqlDatabaseResourceArm.Value;

            return cosmosDBSqlDatabaseResource;

        }
        public async Task<Database> CreateDatbaseAsync(CosmosDBAccountResource cosmosDBAccountResource, string databaseName)
        {
            CosmosClient cosmosClient = CreateCosmosClient(GetAccountConnectionString(cosmosDBAccountResource));
            return await CreateDatbaseAsync(cosmosClient, databaseName);
        }
        public async Task<Database> CreateDatbaseAsync(CosmosClient cosmosClient, string databaseName)
        {
            Database database = await cosmosClient.CreateDatabaseAsync(databaseName);
            return database;
        }
        public void CreateContainer(CosmosDBAccountResource cosmosDBAccountResource, string containerName,string databaseName,string accountName, bool isGremlin = false)
        {
            CosmosClient cosmosClient = CreateCosmosClient(GetAccountConnectionString(cosmosDBAccountResource));
            CreateContainer(cosmosClient, databaseName, containerName, isGremlin);
        }
        public void CreateContainer(CosmosClient cosmosClient, string databaseName, string containerName, bool isGremlin = false)
        {
            Database database = cosmosClient.GetDatabase(databaseName);
            CreateContainer(database, containerName, isGremlin);
        }
        private void CreateContainer(Database database, string containerName, bool isGremlin = false)
        {
            ContainerProperties containerProperties = new ContainerProperties() { Id = containerName, PartitionKeyPath = (isGremlin ? "/_label" : "/id") };
            database.CreateContainerIfNotExistsAsync(containerProperties);
        }
        public void CreateMultipleContainers(CosmosDBAccountResource cosmosDBAccountResource,string databaseName,string accountName, List<string> containerNames, bool isGremlin = false)
        {
            CosmosClient cosmosClient = CreateCosmosClient(GetAccountConnectionString(cosmosDBAccountResource));
            foreach(string containerName in containerNames)
            {
                CreateContainer(cosmosClient, containerName, databaseName, isGremlin);
            }
        }

        public void CreateMultipleContainers(Database database, List<string> containerNames, bool isGremlin = false)
        {
            foreach (string containerName in containerNames)
            {
                CreateContainer(database, containerName, isGremlin);
            }
        }

        public void CreateMultipleContainers(CosmosDBAccountResource cosmosDBAccountResource, List<string> containers, bool isGremlinAccount)
        {
            CreateMultipleContainers(cosmosDBAccountResource, containers, isGremlinAccount);
        }
        public string GetAccountConnectionString(CosmosDBAccountResource cosmosDBAccountResource,string databaseName,string accountName, bool isGremlin)
        {
            string connStr = cosmosDBAccountResource.GetConnectionStrings().Where(item => item.Description == "Primary SQL Connection String").Select(i => i.ConnectionString).First();
            connStr += $";DatabaseName={databaseName}";
            if (isGremlin)
            {
                string gremlinUri = EnvironmentHelper.GetGremlinEndpointUri();
                string port = connStr.Split(";").Where(item => item.Contains("AccountEndpoint")).First().Split(":").Last();
                gremlinUri = gremlinUri.Replace("<<account_name>>", accountName).Replace("<<port>>", port);
                connStr = connStr += $";GremlinEndpoint={gremlinUri}";
            }
            return connStr;
        }
        public string GetAccountConnectionString(CosmosDBAccountResource cosmosDBAccountResource)
        {
            return cosmosDBAccountResource.GetConnectionStrings().Where(item => item.Description == "Primary SQL Connection String").Select(i => i.ConnectionString).First();
        }
        public CosmosClient CreateCosmosClient(string sqlConnectionString)
        {
            CosmosClient cosmosClient = new CosmosClient(sqlConnectionString);
            return cosmosClient;
        }
    }
}
