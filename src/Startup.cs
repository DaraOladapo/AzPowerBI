using AzCostPowerBI.Common;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzCostPowerBI.Startup))]
namespace AzCostPowerBI
{
    public class Startup: FunctionsStartup
    {
          public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAzureCostManagement, AzureCostManagement>();
        }
    }
}