using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace dependency_injection_azure_function
{
    public class WorkController
    {
        private IMyService _myService;

        public WorkController (IMyService myService)
        {
            _myService = myService;
        }

        [FunctionName("work")]
        public async Task<IActionResult> Run (
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            _myService.DoWork();

            return new OkObjectResult("foo");
        }
    }
}