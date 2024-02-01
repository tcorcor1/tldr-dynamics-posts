using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseConsole
{
	internal class Program
	{
		private static IConfiguration _configuration { get; set; }

		private static ServiceClient _service { get; set; }

		private Program ()
		{
			_configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			_service = new ServiceClient(_configuration.GetConnectionString("DATAVERSE_CONN"));
		}

		private static async Task Main (string[] args)
		{
			#region SETUP

			var app = new Program();
			var whoAmIResp = (WhoAmIResponse)_service.Execute(new WhoAmIRequest());
			Console.WriteLine("Logged in User GUID: {0}", whoAmIResp.UserId);

			#endregion SETUP

			var targetEntity = "yyz_foo";
			var pluginType = await GetPluginType(new Guid("010241af-59d8-4255-88a5-f3c75f040018"));
			var sdkMessage = await GetSdkMessage("Update");
			var sdkMessageFilter = await GetSdkMessageFilter(targetEntity, sdkMessage.Id);

			var sdkMessageProcessingStep = new Entity("sdkmessageprocessingstep")
			{
				["name"] = "tldr dynamics UPDATE message",
				["mode"] = new OptionSetValue((int)Mode.Async),
				["rank"] = 1,
				["plugintypeid"] = new EntityReference("plugintype", pluginType.Id),
				["sdkmessageid"] = new EntityReference("sdkmessage", sdkMessage.Id),
				["stage"] = new OptionSetValue((int)Stage.PostOperation),
				["supporteddeployment"] = new OptionSetValue((int)SupportedDeployment.Server),
				["invocationsource"] = new OptionSetValue((int)InvocationSource.Parent),
				["asyncautodelete"] = true,
				["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", sdkMessageFilter.Id),
				["filteringattributes"] = "statecode" // comma-separated list
			};

			var sdkMessageProcessingStepGuid = await _service.CreateAsync(sdkMessageProcessingStep);

			var sdkMessageProcessingStepPostImage = new Entity("sdkmessageprocessingstepimage")
			{
				["name"] = "tldrdynamicsimage",
				["entityalias"] = "tldrdynamicsimage",
				["description"] = "Tldr Dynamics - Demo",
				["imagetype"] = new OptionSetValue((int)ImageType.PostImage),
				["messagepropertyname"] = "Target",
				["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", sdkMessageProcessingStepGuid)
				// If ["attributes"] property is not included, all columns are included in image otherwise provide a comma-separated list
			};

			var postImageGuid = await _service.CreateAsync(sdkMessageProcessingStepPostImage);
		}

		private static Task<Entity> GetPluginType (Guid assemblyId)
		{
			/*
			 * If you need to find your assembly id you can search the web API by assembly name:
			 * https://[yourorg].api.crm.dynamics.com/api/data/v9.2/plugintypes?$filter=name%20eq%20%27Tldr.Demo.Plugin.DemoPlugin%27
			 */
			return _service.RetrieveAsync("plugintype", assemblyId, new ColumnSet("plugintypeid"));
		}

		private static async Task<Entity?> GetSdkMessage (string messageName)
		{
			var qry = new QueryExpression()
			{
				ColumnSet = new ColumnSet(new string[] { "name", "sdkmessageid" }),
				EntityName = "sdkmessage"
			};

			var filter = new FilterExpression(LogicalOperator.And);
			filter.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, messageName));

			qry.Criteria.AddFilter(filter);

			var sdkMessageCollection = await _service.RetrieveMultipleAsync(qry);

			return sdkMessageCollection.Entities.FirstOrDefault();
		}

		private static async Task<Entity?> GetSdkMessageFilter (string entity, Guid sdkMessageId)
		{
			var qry = new QueryExpression()
			{
				ColumnSet = new ColumnSet(new string[] { "primaryobjecttypecode", "sdkmessageid" }),
				EntityName = "sdkmessagefilter"
			};

			var filter = new FilterExpression(LogicalOperator.And);
			filter.AddCondition(new ConditionExpression("primaryobjecttypecode", ConditionOperator.Equal, entity));
			filter.AddCondition(new ConditionExpression("sdkmessageid", ConditionOperator.Equal, sdkMessageId));

			qry.Criteria.AddFilter(filter);

			var sdkMessageFilterCollection = await _service.RetrieveMultipleAsync(qry);

			/*
			 * these records take a moment to be available when you create a new entity but you can double check via the below:
			 * https://[yourcrm].api.crm.dynamics.com/api/data/v9.2/sdkmessagefilters?$filter=primaryobjecttypecode eq 'yyz_foo'
			*/
			return sdkMessageFilterCollection.Entities.FirstOrDefault();
		}
	}
}