using System.Collections.Generic;
using AzCostPowerBI.Models;
using Microsoft.Extensions.Logging;

namespace AzCostPowerBI.Common
{
    public interface IAzureCostManagement
    {
        public List<CostManagementResponse> GetCostManagementData(ILogger log, string subscriptionId, int startDate, int endDate); 
    }
    public interface IAzureBill
    {
        public List<CostManagementResponse> GetCostManagementData(ILogger log, string subscription, string dateFrom, string dateTo);
    }
}