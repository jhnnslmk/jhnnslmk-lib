#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings
// taken from https://forum.unity3d.com/threads/math-problem.8114/#post-59715

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "ClosestPoint", Category = "3d", Version = "Line", Help = "Finds the closest point of a line to another point", Author="dominikkoller")]
	#endregion PluginInfo
	public class Line3dClosestPointNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Line Point 1")]
		public ISpread<Vector3D> FLinePoint1;
		
		[Input("Line Point 2")]
		public ISpread<Vector3D> FLinePoint2;
		
		[Input("Point")]
		public ISpread<Vector3D> FPoint;
		
		[Input("OnSegment")]
		public ISpread<bool> FOnSegment;

		[Output("Closest Point on Line")]
		public ISpread<Vector3D> FClosest;
		
		[Output("Distance to Line")]
		public ISpread<double> FDistance;
		
		[Output("Is On Segment")]
		public ISpread<bool> FIsOnSegment;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FClosest.SliceCount = 
			FDistance.SliceCount = 
			FIsOnSegment.SliceCount = 

			SpreadMax;

			for (int i = 0; i < SpreadMax; i++)
			{
				Vector3D closest;
				double distance;
				bool onSegment;
			
				ClosestPointOnLine(FLinePoint1[i], FLinePoint2[i], FPoint[i], FOnSegment[i], out closest, out distance, out onSegment);
				
				FClosest[i] = closest;
				FDistance[i] = distance;
				FIsOnSegment[i] = onSegment;
			}
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
		
		private void ClosestPointOnLine(Vector3D vA, Vector3D vB, Vector3D vPoint, bool onSegment, out Vector3D outClosest, out double outDistance, out bool outIsOnSegment)
		{
   	var vVector1 = vPoint - vA;
    var vVector2 = (vB - vA);
	vVector2 = ~ vVector2; // normalize
 
    var d = Distance(vA, vB);
    var t = vVector2 | vVector1; // DOT product
 
	outIsOnSegment = (t>0 && t<d);
	
    if (onSegment && t <= 0)
	{
		outClosest = vA;
		outDistance = Distance(vA, vPoint);
		return;
	}
 
    if (onSegment && t >= d)
	{
		outClosest = vB;
		outDistance = Distance(vB, vPoint);	
		return;
	}

    var vVector3 = vVector2 * t;
 
    var vClosestPoint = vA + vVector3;
 
	outClosest = vClosestPoint;
	outDistance = Distance(vClosestPoint, vPoint);
}
		
		double Distance(Vector3D v1, Vector3D v2)
		{
			var v = v1 - v2;
			return v.Length;
		}
	}
}
