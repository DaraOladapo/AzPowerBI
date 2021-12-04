using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzCostPowerBI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace AzCostPowerBI.Common
{
    public class AzureCostManagement : IAzureCostManagement
    {
        // Cost Mgmt Service Principal
        private string _costMgmtSpClientId = Environment.GetEnvironmentVariable("CostMgmt_SP_ClientId");
        private string _costMgmtSpClientSecret = Environment.GetEnvironmentVariable("CostMgmt_SP_ClientSecret");
        private string _costMgmtAuthUrl = "https://login.microsoft.com/" + Environment.GetEnvironmentVariable("CostMgmt_SP_TenantId");
        private string _subscriptionId;
        public string GetCostManagementDataResponse(ILogger log, string subscriptionId, int startDate, int endDate)
        {
            _subscriptionId = subscriptionId;
            var invoiceDayFrom = startDate;
            var invoiceDayTo = endDate;
            var costManagementData = new List<CostManagementResponse>();

            var authenticationContext = new AuthenticationContext(_costMgmtAuthUrl);
            var credential = new ClientCredential(clientId: _costMgmtSpClientId, clientSecret: _costMgmtSpClientSecret);
            var result =authenticationContext.AcquireTokenAsync(resource: "https://management.azure.com/", clientCredential: credential).Result;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            // Format the request to the API
            // Note - as of 24/08/2021 there is no SDK for the CostManagement functionality in Azure
            // so this data has to be obtained via the API used from inside the Azure portal
            Uri uri = new Uri(String.Format($"https://management.azure.com/subscriptions/{_subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01&$top=5000"));

            log.LogInformation($"Getting 3 months of usage data from {uri} ...");

            // Get three months worth of data
            var timeRangeQueries = new List<List<string>>
            {
                new List<string> {
                    $"{DateTime.Now.Year}{DateTime.Now.Month.ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-1}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month}-{invoiceDayTo})"
                },
                new List<string> {
                    $"{DateTime.Now.Year}{(DateTime.Now.Month-1).ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-2}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month-1}-{invoiceDayTo})"
                },
                new List<string> {
                    $"{DateTime.Now.Year}{(DateTime.Now.Month-2).ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-3}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month-2}-{invoiceDayTo})"
                },
            };

            string webResponse = string.Empty;
            // Fire 3 queries - this could be improved but this is a PoC
            foreach (var timeRangeQuery in timeRangeQueries)
            {
                var body = new CostManagementQuery(timeRangeQuery);
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    string jsonTransport = JsonConvert.SerializeObject(body);
                    var jsonPayload = new StringContent(jsonTransport, Encoding.UTF8, "application/json");
                    var httpResponse = httpClient.PostAsync(uri, jsonPayload).Result;
                    webResponse = httpResponse.Content.ReadAsStringAsync().Result;
                }
            }
            return webResponse;

        }
        public List<CostManagementResponse> GetCostManagementData(ILogger log, string subscriptionId, int startDate, int endDate)
        {
           _subscriptionId = subscriptionId;
           var invoiceDayFrom = startDate;
           var invoiceDayTo = endDate;
           var costManagementData = new List<CostManagementResponse>();

           var authenticationContext = new AuthenticationContext(_costMgmtAuthUrl);
           var credential = new ClientCredential(clientId: _costMgmtSpClientId, clientSecret: _costMgmtSpClientSecret);
           var result = authenticationContext.AcquireTokenAsync(resource: "https://management.azure.com/", clientCredential: credential).Result;

           if (result == null)
           {
               throw new InvalidOperationException("Failed to obtain the JWT token");
           }

           // Format the request to the API
           // Note - as of 24/08/2021 there is no SDK for the CostManagement functionality in Azure
           // so this data has to be obtained via the API used from inside the Azure portal
           Uri uri = new Uri(String.Format($"https://management.azure.com/subscriptions/{_subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01&$top=5000"));

           log.LogInformation($"Getting 3 months of usage data from {uri} ...");

           // Get three months worth of data
           var timeRangeQueries = new List<List<string>>
           {
               new List<string> {
                   $"{DateTime.Now.Year}{DateTime.Now.Month.ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-1}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month}-{invoiceDayTo})"
               },
               new List<string> {
                   $"{DateTime.Now.Year}{(DateTime.Now.Month-1).ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-2}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month-1}-{invoiceDayTo})"
               },
               new List<string> {
                   $"{DateTime.Now.Year}{(DateTime.Now.Month-2).ToString("d2")}({DateTime.Now.Year}-{DateTime.Now.Month-3}-{invoiceDayFrom} - {DateTime.Now.Year}-{DateTime.Now.Month-2}-{invoiceDayTo})"
               },
           };

           // Fire 3 queries - this could be improved but this is a PoC
           foreach (var timeRangeQuery in timeRangeQueries)
           {
               //// Create the request - emulates the Azure portal request
               //var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
               //httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + result.AccessToken);
               //httpWebRequest.ContentType = "application/json";
               //httpWebRequest.Method = "POST";

               var body = new CostManagementQuery(timeRangeQuery);

               // Post the request
               //try
               //{
               //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
               //    {
               //        streamWriter.Write(JsonConvert.SerializeObject(body));
               //        streamWriter.Flush();
               //        streamWriter.Close();
               //    }
               //}
               //catch (Exception ex)
               //{
               //    log.LogInformation($"Error getting Azure Cost Data from : {uri} : { ex.Message }");
               //}
               string webResponse = string.Empty;
               using (HttpClient httpClient = new HttpClient())
               {
                   httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                   string jsonTransport = JsonConvert.SerializeObject(body);
                   var jsonPayload = new StringContent(jsonTransport, Encoding.UTF8, "application/json");
                   var httpResponse =httpClient.PostAsync(uri, jsonPayload).Result;
                   webResponse = httpResponse.Content.ReadAsStringAsync().Result;
               }
               // Get the response
               //HttpWebResponse httpResponse = null;
               //try
               //{
               //    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
               //}
               //catch (Exception ex)
               //{
               //    log.LogInformation($"Error getting Azure Cost Data from : {uri} : { ex.Message }");
               //}

               CostManagementResponse formattedResponse;


               var costMgmtResult = webResponse;
               formattedResponse = JsonConvert.DeserializeObject<CostManagementResponse>(costMgmtResult);
               formattedResponse.properties.rowsConverted = new List<CostManagementResponse.CostManagementQueryResultRow>();

               foreach (var row in formattedResponse.properties.rows)
               {
                   var rowList = row.Select(i => i.ToString()).ToList();

                   // Note - purposefully ignored some properties as not all are needed for the automation
                   formattedResponse.properties.rowsConverted.Add(new CostManagementResponse.CostManagementQueryResultRow
                   {
                       Cost = rowList.Count >= 1 ? float.Parse(rowList[0]) : 0,
                       CostUSD = rowList.Count >= 2 ? float.Parse(rowList[1]) : 0,
                       ResourceGroupName = rowList.Count >= 5 ? rowList[4] : string.Empty,
                       Currency = rowList.Count >= 7 ? rowList[6] : string.Empty
                   });
               }

               formattedResponse.Subscription = subscriptionId;
               formattedResponse.DateQuery = timeRangeQuery[0];
               formattedResponse.TotalCost = formattedResponse.properties.rowsConverted.Sum(x => x.Cost);

               log.LogInformation($"Usage data retrieved for {formattedResponse.DateQuery}, total cost = {formattedResponse.TotalCost}.");


               costManagementData.Add(formattedResponse);
           }

           return costManagementData;
        }
    }
}