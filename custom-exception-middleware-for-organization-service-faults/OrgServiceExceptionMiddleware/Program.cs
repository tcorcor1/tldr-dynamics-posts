using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerPlatform.Dataverse.Client;
using OrgServiceExceptionMiddleware;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication(builder =>
	{
		builder.UseMiddleware<ExceptionHandlerMiddleware>();
	})
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddScoped<IOrganizationServiceAsync>(srv => new ServiceClient(Environment.GetEnvironmentVariable("DATAVERSE_CONNECTION")));
	})
	.Build();

host.Run();