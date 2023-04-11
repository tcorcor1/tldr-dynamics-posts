using System;
using System.Xml;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace Tldr.Common
{
	public abstract class PluginBase : IPlugin
	{
		private string _unsecureConfig;
		private string _secureConfig;

		public abstract void ExecuteAction (ExecutionContext ctx);

		public PluginBase ()
		{
		}

		public PluginBase (string UnsecureConfig, string SecureConfig)
		{
			this._unsecureConfig = UnsecureConfig;
			this._secureConfig = SecureConfig;
		}

		public void Execute (IServiceProvider serviceProvider)
		{
			ExecuteAction(new ExecutionContext(serviceProvider, _unsecureConfig, _secureConfig));
		}
	}

	public class ExecutionContext
	{
		public IPluginExecutionContext PluginContext;
		public IOrganizationService Service;
		public ITracingService TracingService;
		public ILogger Logger;

		public PluginConfig UnsecureConfig;
		public PluginConfig SecureConfig;

		public Entity Target => (Entity)PluginContext.InputParameters["Target"];
		public Entity PreImage => PluginContext.PreEntityImages.Values.FirstOrDefault();
		public Entity PostImage => PluginContext.PostEntityImages.Values.FirstOrDefault();

		public ExecutionContext (IServiceProvider serviceProvider, string unsecureConfig, string secureConfig)
		{
			PluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
			Logger = (ILogger)serviceProvider.GetService(typeof(ILogger));

			var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
			Service = serviceFactory.CreateOrganizationService(null);

			UnsecureConfig = new PluginConfig(unsecureConfig);
			SecureConfig = new PluginConfig(secureConfig);
		}
	}

	public class PluginConfig
	{
		private readonly string _pluginConfig;
		private readonly XmlDocument _xmlDocument;

		public PluginConfig (string config)
		{
			if (string.IsNullOrEmpty(config))
			{
				return;
			}
			_pluginConfig = config;
			_xmlDocument = new XmlDocument();
			try
			{
				_xmlDocument.LoadXml(_pluginConfig);
			}
			catch
			{
				_xmlDocument = null;
			}
		}

		public string GetSetting (string attributeName)
		{
			if (_xmlDocument == null)
			{
				return null;
			}
			var settingsNode = _xmlDocument.SelectSingleNode("//root/settings");
			var settingAttribute = settingsNode?.Attributes?[attributeName];
			return settingAttribute?.Value;
		}
	}
}