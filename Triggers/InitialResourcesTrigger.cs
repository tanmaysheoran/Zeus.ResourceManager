using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.ResourceManager.Models;
using Zeus.ResourceManager.Services;

namespace Zeus.ResourceManager.Triggers
{
    public class InitialResourcesTrigger
    {
        [FunctionName("InitialResourcesAsync")]
        public static async Task<IActionResult> InitialResourcesAsync(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateInitialResourcesModel data = JsonConvert.DeserializeObject<CreateInitialResourcesModel>(requestBody);
            data.cosmosAccount.isGremlinAccount = false;
            data.gremlinAccount.isGremlinAccount = true;
            InitialResourcesService initialResourcesService = new InitialResourcesService();
            initialResourcesService.InitialResourcesAsync(data);
            return new AcceptedResult();
        }

        [FunctionName("InitialResources")]
        public static async Task<IActionResult> InitialResources(
[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<CreateInitialResourcesModel>(requestBody);
            InitialResourcesService initialResourcesService = new InitialResourcesService();
            await initialResourcesService.InitialResourcesAsync(data);
            return new OkResult();
        }
    }
}
