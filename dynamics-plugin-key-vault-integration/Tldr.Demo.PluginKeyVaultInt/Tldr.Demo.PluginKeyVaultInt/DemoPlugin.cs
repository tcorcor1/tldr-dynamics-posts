using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tldr.Common;

namespace Tldr.Demo.PluginKeyVaultInt
{
	public class DemoPlugin : PluginBase
	{
		public override void ExecuteAction (ExecutionContext ctx)
		{
			var secretMessage = ctx.GetEnvironmentVariable("yyz_tldrdemomessage");

			var newNote = new Entity("annotation")
			{
				["notetext"] = secretMessage
			};

			var newNoteId = ctx.Service.Create(newNote);
		}
	}
}