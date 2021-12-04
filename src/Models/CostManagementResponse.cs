using System.Collections.Generic;

namespace AzCostPowerBI.Models
{ /// <summary>
    /// https://docs.microsoft.com/en-us/rest/api/cost-management/query/usage#querydataset
    /// </summary>
    public class CostManagementResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public object location { get; set; }
        public object sku { get; set; }
        public object eTag { get; set; }
        public Properties properties { get; set; }

        // Extended attributes
        public string Subscription { get; set; }
        public string DateQuery { get; set; }
        public float TotalCost { get; set; }

        public class Column
        {
            public string name { get; set; }
            public string type { get; set; }
        }

        public class Properties
        {
            public string nextLink { get; set; }
            public List<Column> columns { get; set; }
            public List<List<object>> rows { get; set; }

            // Extended attribute
            public List<CostManagementQueryResultRow> rowsConverted { get; set; } 
        }

        public class CostManagementQueryResultRow
        {
            public float Cost { get; set; }
            public float CostUSD { get; set; }
            public string ResourceId { get; set; }
            public string ResourceType { get; set; }
            public string ResourceLocation { get; set; }
            public string ChargeType { get; set; }
            public string ResourceGroupName { get; set; }
            public string PublisherType { get; set; }
            public string ServiceName { get; set; }
            public string ServiceTier { get; set; }
            public string Meter { get; set; }
            public List<string> Tags { get; set; }
            public string Currency { get; set; }
        }
    }
}
