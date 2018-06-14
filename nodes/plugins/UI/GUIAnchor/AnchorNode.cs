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
	[PluginInfo(
	Name = "Anchor",
	Category = "UI",
	Help = "",
	Tags = "Alignment Offset")]
	#endregion PluginInfo
	public class AnchorNode : IPluginEvaluate
	{
		#region fields & pins
		
		[Input("Canvas", Order = 0)]
		public ISpread<UI.Canvas> FInCanvas;
		
		[Input("Size", DefaultValues = new double[]{1,1}, Order = 1)]
		public ISpread<Vector2D> FInSize;
		
		[Input("Offset", Order = 2)]
		public ISpread<Vector2D> FInOffset;
		
		[Input("Parent", Order = 3)]
        public IDiffSpread<UI.Alignment> FInParentAlign;
		
		[Input("Element", Order = 4)]
        public IDiffSpread<UI.Alignment> FInElementAlign;
		
		[Input("Resolution", EnumName = "ResolutionManager", Order = 5)]
        public IDiffSpread<EnumEntry> FInResolution;
		
		[Output("Transform")]
		public ISpread<Matrix4x4> FOutTransform;
		
		[Output("Canvas")]
		public ISpread<UI.Canvas> FOutCanvas;
		
		[Import()]
		public ILogger FLogger;
		
		#endregion fields & pins
		
		#region constructor and init
		
		public AnchorNode()
		{
			
		}
		
		#endregion constructor and init
		
		public void Evaluate(int SpreadMax)
		{
			FOutTransform.SliceCount = SpreadMax;
			FOutCanvas.SliceCount = SpreadMax;
			
			for (int i = 0; i < SpreadMax; i++)
			{
				Vector2D _element = UI.AlignmentVectors[(int)FInElementAlign[i]];
				Vector2D _parent = UI.AlignmentVectors[(int)FInParentAlign[i]];
				
				UI.Canvas c = FInCanvas[i];
				if (c == null) c = new UI.Canvas(2 * VMath.IdentityMatrix);
				Matrix4x4 m;
				
				m = VMath.Translate(new Vector3D(-1*_element*0.5f, 0));
				
				m *= VMath.Scale(new Vector3D(ResolutionManager.GetSize(FInResolution[0].Name) * FInSize[i], 1));
//				FLogger.Log(LogType.Debug, FInResolution[0].Name);
				
				m *= VMath.Translate(c.Center);
				
				Vector2D offsetDirection = (FInParentAlign[i] == UI.Alignment.CenterCenter) ? new Vector2D(-1, -1) : _element;
				offsetDirection = new Vector2D(-1, -1);
				m *= VMath.Translate(new Vector3D((_parent*0.5f*c.Size-offsetDirection*ResolutionManager.GetSize(FInResolution[0].Name) * FInOffset[i]), 0));
				
				FOutTransform[i] = m;
				FOutCanvas[i] = new UI.Canvas(m);
			}
		}
	}
}
