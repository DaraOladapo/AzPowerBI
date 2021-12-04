using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzCostPowerBI.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DaraOladapo
{
    public class CostManagementRaw
    {
        private readonly ILogger<CostManagementRaw> _logger;
        private readonly IAzureCostManagement _azureCostManagement;

        public CostManagementRaw(ILogger<CostManagementRaw> log, IAzureCostManagement azureCostManagement)
        {
            _logger = log;
            _azureCostManagement = azureCostManagement;
        }


        [FunctionName("CostManagementRaw")]
               [OpenApiOperation(operationId: "Run", tags: new[] { "startDate","endDate" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string startDate = req.Query["startDate"];
            string endDate = req.Query["endDate"];
            var subscriptionId=Environment.GetEnvironmentVariable("SubscriptionId");

            // Get the cost management data from Azure
            var azureCostData = _azureCostManagement.GetCostManagementDataResponse(_logger, subscriptionId, int.Parse(startDate), int.Parse(endDate));
            string responseMessage = JsonConvert.SerializeObject(azureCostData);

            return new OkObjectResult(azureCostData);
        }
    }
}

