using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(dependency_injection_azure_function.Startup))]

namespace dependency_injection_azure_function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure (IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("NHL-API", c =>
            {
                c.BaseAddress = new Uri("https://statsapi.web.nhl.com");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddScoped<IMyService, MyService>();
        }
    }
}