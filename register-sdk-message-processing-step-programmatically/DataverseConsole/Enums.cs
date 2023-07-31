using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseConsole
{
	public enum Mode
	{
		Sync = 0,
		Async = 1
	}

	public enum SupportedDeployment
	{
		Server = 0,
		Offline = 1,
		Both = 2
	}

	public enum InvocationSource
	{
		Parent = 0,
		Child = 1
	}

	public enum Stage
	{
		PreValidation = 10,
		PreOperation = 20,
		PostOperation = 40
	}

	public enum StatusCode
	{
		Enabled = 1,
		Disabled = 2
	}

	public enum StateCode
	{
		Enabled = 0,
		Disabled = 1
	}

	public enum ImageType
	{
		PreImage = 0,
		PostImage = 1,
		Both = 2
	}
}