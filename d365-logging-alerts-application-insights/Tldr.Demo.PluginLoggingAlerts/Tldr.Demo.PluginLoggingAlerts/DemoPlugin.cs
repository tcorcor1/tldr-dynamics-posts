using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tldr.Common;

namespace Tldr.Demo.PluginLoggingAlerts
{
	public class DemoPlugin : PluginBase
	{
		public override void ExecuteAction (ExecutionContext ctx)
		{
			ctx.Logger.LogInformation("TLDR DEMO: This is a log written to app insights");

			ctx.Logger.LogError("TLDR DEMO: This is an error message written to app insights");
		}
	}
}