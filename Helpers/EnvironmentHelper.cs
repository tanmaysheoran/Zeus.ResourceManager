using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.ResourceManager.Helpers
{
    public class EnvironmentHelper
    {

        public static string GetSubscriptionId()
        {
            return Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
        }
        public static string GetGremlinEndpointUri()
        {
            return Environment.GetEnvironmentVariable("GremlinEndpointURI");
        }
        public static string GetCurrentEnv()
        {
            return Environment.GetEnvironmentVariable("ENV");
        }
        public static string GetIdentityGraphConnectionString()
        {
            return Environment.GetEnvironmentVariable("IDENTITY_GRAPH_CONN_STR");
        }

    }
}
