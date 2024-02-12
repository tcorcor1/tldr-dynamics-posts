using System.Net;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace OrgServiceExceptionMiddleware
{
	public class ExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
	{
		public async Task Invoke (FunctionContext context, FunctionExecutionDelegate next)
		{
			try
			{
				await next.Invoke(context);
			}
			catch (Exception exception)
			{
				var logger = context.GetLogger<ExceptionHandlerMiddleware>();

				var req = await context.GetHttpRequestDataAsync();
				var res = req!.CreateResponse();

				switch (exception)
				{
					case BadHttpRequestException ex:
						logger.LogError(exception.Message);

						req = await context.GetHttpRequestDataAsync();
						res = req!.CreateResponse();
						res.StatusCode = HttpStatusCode.InternalServerError;

						await res.WriteStringAsync(ex.Message);
						context.GetInvocationResult().Value = res;
						break;

					case FaultException<OrganizationServiceFault> ex:
						logger.LogCritical(((FaultException<OrganizationServiceFault>)exception).Detail.Message);

						req = await context.GetHttpRequestDataAsync();
						res = req!.CreateResponse();
						res.StatusCode = HttpStatusCode.InternalServerError;

						await res.WriteStringAsync("Org service fault. Please try again or contact an administrator");
						context.GetInvocationResult().Value = res;
						break;

					default:
						logger.LogError(exception.Message);

						req = await context.GetHttpRequestDataAsync();
						res = req!.CreateResponse();
						res.StatusCode = HttpStatusCode.InternalServerError;

						await res.WriteStringAsync("Internal service error. Please contact an administrator");
						context.GetInvocationResult().Value = res;
						break;
				}
			}
		}
	}
}