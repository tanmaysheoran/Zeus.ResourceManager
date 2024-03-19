using Athena.DatabaseHelper.Models;
using Athena.DatabaseHelper.Services;
using Gremlin.Net.Driver;
using System.Threading.Tasks;

namespace Zeus.ResourceManager.Helpers
{
    public class GremlinHelper
    {

        public static GremlinClient GetIdentityGremlinClient(GremlinService gremlinService)
        {
            string connStr = EnvironmentHelper.GetIdentityGraphConnectionString();
            GremlinAccount gremlinAccount = new GremlinAccount(connStr);
            var gremlinClient = gremlinService.CreateGremlinClient(gremlinAccount);
            return gremlinClient;
        }

        public static async Task<bool> AddConnectionStringToIdentity(string tenantId, string storageConnStr, string cosmosConnStr, string gremlinConnStr, GremlinClient gremlinClient, GremlinService gremlinService)
        {
            string query = $"g.V('{tenantId}').addE('has_databaseinfo').to(addV('databaseinfo').property('_label','databaseinfo').property('storageAccount','{storageConnStr}').property('cosmosAccount','{cosmosConnStr}').property('gremlinAccount','{gremlinConnStr}'))";
            var result = await gremlinService.GremlinExecutorAsync(gremlinClient, query);

            if(result.StatusCode == 200)
                return true;
            else
                return false;
        }

        public static async Task<bool> DeleteTenantRecordOnFail(string tenantId, GremlinClient gremlinClient, GremlinService gremlinService)
        {
            string query = $"g.V('{tenantId}').drop()";
            var result = await gremlinService.GremlinExecutorAsync(gremlinClient, query);

            if (result.StatusCode == 200)
                return true;
            else
                return false;
        }

    }
}
