#region usings
using System;
using System.ComponentModel.Composition;

//using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

//using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "HostName", Category = "System", Help = "Outputs the computer's hostname", Tags = "system")]
	#endregion PluginInfo
	public class SystemHostNameNode : IPluginEvaluate, IPartImportsSatisfiedNotification
	{
		#region fields & pins
		[Output("HostName")]
		public ISpread<string> FOutHostName;
		
		#endregion fields & pins
		
		public void OnImportsSatisfied()
		{
			FOutHostName.SliceCount = 1;
			FOutHostName[0] = Environment.MachineName;
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{

		}
	}
}
