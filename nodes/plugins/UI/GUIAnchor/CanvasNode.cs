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
	[PluginInfo(Name = "Canvas", Category = "UI", Help = "Basic template with one value in/out", Tags = "c#")]
	#endregion PluginInfo
	public class UICanvasNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Size", DefaultValues = new double[]{1,1})]
		public ISpread<Vector2D> FInSize;
		
		[Input("Offset", DefaultValues = new double[]{0,0})]
		public ISpread<Vector2D> FInOffset;
		
		[Output("Output")]
		public ISpread<UI.Canvas> FOutput;
		
		[Output("Transform")]
		public ISpread<Matrix4x4> FOutTransform;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			FOutTransform.SliceCount = SpreadMax;
			
			for (int i = 0; i < SpreadMax; i++)
			{
				Matrix4x4 m;
				m = VMath.Scale(new Vector3D(FInSize[i], 1));
				m *= VMath.Translate(new Vector3D(FInOffset[i], 0));
				FOutput[i] = new UI.Canvas(m);
				FOutTransform[i] = m;
			}
			
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
