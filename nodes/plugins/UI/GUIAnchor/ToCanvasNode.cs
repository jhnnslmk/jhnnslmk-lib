#region usings
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes.UI
{
	#region PluginInfo
	[PluginInfo(Name = "ToCanvas", Category = "UI", Help = "Converts a Matrix to Canvas Type", Tags = "Transform Canvas UI")]
	#endregion PluginInfo
	public class ToCanvasNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Size")]
		public ISpread<Matrix4x4> FInTransform;
		
		[Output("Output")]
		public ISpread<UI.Canvas> FOutput;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			
			for (int i = 0; i < SpreadMax; i++)
			{
				FOutput[i] = new UI.Canvas(FInTransform[i]);
			}
			
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
