#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using System.Linq;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "FillUp", Category = "Value")]
	#endregion PluginInfo
	public class ValueFillUpNode : FillUpNode<double> { }
	
	#region PluginInfo
	[PluginInfo(Name = "FillUp", Category = "String")]
	#endregion PluginInfo
	public class StringFillUpNode : FillUpNode<string> { }
	
	public class FillUpNode<T> : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultString = "Text")]
		public ISpread<ISpread<T>> FInput;
		
		[Input("Fill Value", DefaultString = "Fill")]
		public ISpread<T> FInFillValue;
		
		[Input("Spread Count", DefaultValue = 1)]
		public ISpread<int> FInSpreadCount;

		[Output("Output")]
		public ISpread<ISpread<T>> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			var spreadcount = (FInput == null ||FInSpreadCount == null || FInFillValue == null) ? 0 : FInSpreadCount.SliceCount;
			FOutput.SliceCount = spreadcount;

			for (int i = 0; i < spreadcount; i++)
			{
				var count = FInSpreadCount[i];
				FOutput[i].AssignFrom(Enumerable.Repeat(FInFillValue[i], count).ToList());
				
				var loop = Math.Min(FInput[i].SliceCount, count);
				for (int j = 0; j < loop; j++)
				{
					FOutput[i][j] = FInput[i][j];
				}
				
			}

			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
