#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "Toggle", Category = "Animation", Version = "Advanced", Help = "")]
	#endregion PluginInfo
	public class AdvancedAnimationToggleNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Toggle", IsBang = true, Order = 0)]
		public IDiffSpread<bool> FInToggle;
		
		[Input("Reset Value", Order = 1)]
		public IDiffSpread<bool> FInResetValue;
		
		[Input("Reset", IsBang = true, Order = 2)]
		public IDiffSpread<bool> FInReset;
		
		[Output("Output")]
		public ISpread<bool> FOutput;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			
			if (FInToggle.IsChanged || FInReset.IsChanged)
			{
				for (int i = 0; i < SpreadMax; i++)
				{
					if (FInToggle[i])
					{
						FOutput[i] = !FOutput[i];
					}
					if (FInReset[i])
					{
						FOutput[i] = FInResetValue[i];
					}
				}
			}
			
			
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
