using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.ResourceManager.Helpers;
using Zeus.ResourceManager.Models;

namespace Zeus.ResourceManager.Services
{
    public class StorageAccountService
    {
        public static async Task<string> InitializeStorageAccountAsync(StorageAccountModel storageAccountModel)
        {
            StorageAccountResource accountResource = await StorageHelper.CreateNewStorageAccountAsync(storageAccountModel.ResourceGroup,storageAccountModel.AccountType,storageAccountModel.AccountName,storageAccountModel.Location,storageAccountModel.ContainerConfigs);
            string connStr = StorageHelper.GetAccountConnectionString(accountResource, storageAccountModel.AccountName);
            return connStr;
        }
    }
}
