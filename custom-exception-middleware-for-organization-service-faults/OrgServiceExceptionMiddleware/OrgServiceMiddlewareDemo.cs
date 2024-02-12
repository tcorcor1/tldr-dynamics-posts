using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace OrgServiceExceptionMiddleware
{
	public class OrgServiceMiddlewareDemo
	{
		private readonly ILogger<OrgServiceMiddlewareDemo> _logger;
		private readonly IOrganizationServiceAsync _service;

		public OrgServiceMiddlewareDemo (ILogger<OrgServiceMiddlewareDemo> logger, IOrganizationServiceAsync service)
		{
			_logger = logger;
			_service = service;
		}

		[Function("CreateAccount")]
		public async Task<IActionResult> RunCreateAccount ([HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts")] HttpRequest req, string companyName)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			try
			{
				var newAccount = new Entity("account");
				newAccount.Attributes.Add("name", companyName);

				var createAccountRequest = new OrganizationRequest("Create")
				{
					Parameters =
				{
					{ "Target", newAccount}
				}
				};

				var createAccountResponse = await _service.ExecuteAsync(createAccountRequest);

				return new CreatedResult("accounts", $"Created account: {companyName}");
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}