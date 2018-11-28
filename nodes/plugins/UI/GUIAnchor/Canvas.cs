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
	public class UI
	{
		public static Vector2D[] AlignmentVectors =
		{
			new Vector2D(-1,1), new Vector2D(0,1), new Vector2D(1,1),
			new Vector2D(-1,0), new Vector2D(0,0), new Vector2D(1,0),
			new Vector2D(-1,-1), new Vector2D(0,-1), new Vector2D(1,-1)
		};
		
		public enum Alignment
		{
			LeftTop, CenterTop, RightTop,
			LeftCenter, CenterCenter, RightCenter,
			LeftBottom, CenterBottom, RightBottom
		}
		
		public enum AspectRatio
		{
			None,
			FitIn,
			FitOut
		}
		
		public class Canvas
		{
			public Matrix4x4 Matrix;
			public Vector3D Center;
			public Vector2D Size;
			
			public Canvas(Matrix4x4 m)
			{
				Matrix = m;
				Center = m * Vector3D.Zero;
				Size = new Vector2D(m.m11, m.m22);
			}
		}
	}
	
	
}
