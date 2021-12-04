using System.Collections.Generic;

namespace AzCostPowerBI.Models
{/// <summary>
    /// https://docs.microsoft.com/en-us/rest/api/cost-management/query/usage#querydataset
    /// </summary>
    public class CostManagementQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CostManagementQuery"/> class.
        /// </summary>
        /// <param name="timeRangeQuery">The time range for the query. Formatted as YYYYMM(YY-DD-MM - YY-DD-MM).</param>
        public CostManagementQuery(List<string> timeRangeQuery)
        {
            type = "ActualCost";
            dataSet = new DataSet
            {
                granularity = "None",
                aggregation = new Aggregation
                {
                    totalCost = new TotalCost
                    {
                        name = "Cost",
                        function = "Sum"
                    },
                    totalCostUSD = new TotalCostUSD
                    {
                        name = "CostUSD",
                        function = "Sum"
                    },
                },
                grouping = new List<Grouping>
                {
                    new Grouping { type = "Dimension", name = "ResourceId" },
                    new Grouping { type = "Dimension", name = "ChargeType" },
                    new Grouping { type = "Dimension", name = "ResourceGroupName" },
                    new Grouping { type = "Dimension", name = "Meter" },
                },
                filter = new Filter
                {
                    And = new List<And>
                    {
                        new And
                        {
                            Dimensions = new Dimensions
                            {
                                Name = "PublisherType",
                                Operator = "In",
                                Values = new List<string>
                                {
                                    "azure"
                                }
                            }
                        },
                        new And
                        {
                            Dimensions = new Dimensions
                            {
                                Name = "BillingPeriod",
                                Operator = "In",
                                Values = timeRangeQuery
                            }
                        },
                    }
                }
            };
        }

        public string type { get; set; }
        public DataSet dataSet { get; set; }

        public class TotalCost
        {
            public string name { get; set; }
            public string function { get; set; }
        }

        public class TotalCostUSD
        {
            public string name { get; set; }
            public string function { get; set; }
        }

        public class Aggregation
        {
            public TotalCost totalCost { get; set; }
            public TotalCostUSD totalCostUSD { get; set; }
        }

        public class Grouping
        {
            public string type { get; set; }
            public string name { get; set; }
        }

        public class Dimensions
        {
            public string Name { get; set; }
            public string Operator { get; set; }
            public List<string> Values { get; set; }
        }
        public class And
        {
            public Dimensions Dimensions { get; set; }
        }

        public class Filter
        {
            public List<And> And { get; set; }
        }
        public class DataSet
        {
            public string granularity { get; set; }
            public Aggregation aggregation { get; set; }
            public List<Grouping> grouping { get; set; }
            public List<string> include { get; set; }
            public Filter filter { get; set; }
        }
    }
}
