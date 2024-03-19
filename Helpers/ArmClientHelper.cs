using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System;

namespace Zeus.ResourceManager.Helpers
{
    public class ArmClientHelper
    {

        private DefaultAzureCredential GetDefaultAzureCredential()
        {
            var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions {ExcludeVisualStudioCodeCredential = true, ExcludeVisualStudioCredential = true} );
            return credentials;
        }

        public ArmClient CreateArmClient(string subscriptionId)
        {
            var credentials = GetDefaultAzureCredential();
            var client = new ArmClient(credentials, subscriptionId);
            return client;
        }

        public SubscriptionResource GetSubscriptionResource(ArmClient armClient)
        {
            var subscription = armClient.GetDefaultSubscription();
            return subscription;
        }

    }
}
