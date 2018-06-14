#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V2;
using System.Collections.Generic;
using VVVV.Utils.VMath;
using System.Linq;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes.TouchHandling
{
	public class Touch
	{
		public int ID;
		public int Frame;
		public Vector2D IntitialXY;
		public Vector2D LastFrameXY;
		public double Distance;
		public Vector2D DeltaXY;
		public bool Alive;
		public string Timestamp;
	}
	
	#region PluginInfo
	[PluginInfo(Name = "TouchHandling", Category = "Touch", Help = "", Tags = "", Author = "readme, woei")]
	#endregion PluginInfo
	
	public class TouchHandlingNode : IPluginEvaluate
	{
		
		#region fields & pins
		[Input("ID", Order = 0)]
		public ISpread<int> FInput;
		
		[Input("Reset", IsBang = true, IsSingle = true, Order = 2)]
		public ISpread<bool> FReset;
		
		[Input("Touch", Order = 1)]
		public ISpread<Vector2D> FInXY;
		
		[Output("ID", Order = 0)]
		public ISpread<int> FOutID;
		
		[Output("Last Known", Order = 1)]
		public ISpread<Vector2D> FOutLastXY;
		
		[Output("Is New", Order = 2)]
		public ISpread<bool> FOutIsNew;
		
		[Output("Is Gone", IsBang = true, Order = 3)]
		public ISpread<bool> FOutIsGone;
		
		[Output("Timestamp", Order = 4)]
		public ISpread<string> FOutTimestamp;
		
		[Output("Frame Count", Order = 5)]
		public ISpread<int> FAge;
		
		[Output("Start", Order = 6)]
		public ISpread<Vector2D> FOutStartXY;
		
		[Output("Delta", Order = 7)]
		public ISpread<Vector2D> FOutDeltaXY;
		
		[Output("Distance Travelled", Order = 8)]
		public ISpread<double> FOutDistance;
		
		[Output("Touch", Order = 9)]
		public ISpread<VVVV.Nodes.TouchHandling.Touch> FOutTouch;
		
		[Import()]
		public ILogger FLogger;
		
		private Dictionary<int, Touch> touchDict = new Dictionary<int, Touch>();
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (FReset[0])
			{
				touchDict.Clear();
			}
			
			//check which keys aren't available anymore (gone in this frame)
			List<int> deadKeys = touchDict.Keys.Except(FInput).ToList();
			foreach (var deadKey in deadKeys)
			{
				touchDict[deadKey].Alive = false;
			}
			
			for (int i = 0; i < FInput.SliceCount; i++)
			{
				// Update Touches
				if (touchDict.ContainsKey(FInput[i]))
				{
					//age ++
					var t = touchDict[FInput[i]];
					t.Frame = t.Frame + 1;
					t.DeltaXY = FInXY[i] - t.LastFrameXY;
					t.Distance = t.Distance + t.DeltaXY.Length;
					t.LastFrameXY = FInXY[i]; //is [i] really correct at all times!? is that not the point of this plug in ...
					t.Alive = true;
					
					touchDict[FInput[i]] = t;
				}
				// Create New Touches;
				else
				{
					var t = new Touch();
					t.Frame = 0;
					t.IntitialXY = FInXY[i];
					t.LastFrameXY = FInXY[i];
					t.Distance = 0;
					t.DeltaXY = new Vector2D(0,0);
					t.Alive = true;
					t.ID = FInput[i];
					t.Timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
					
					touchDict[FInput[i]] = t;
				}
			}
			
			FOutTouch.SliceCount =
			FOutIsNew.SliceCount =
			FAge.SliceCount =
			FOutID.SliceCount =
			FOutStartXY.SliceCount =
			FOutDeltaXY.SliceCount =
			FOutDistance.SliceCount =
			FOutIsGone.SliceCount =
			FOutLastXY.SliceCount =
			FOutTimestamp.SliceCount = touchDict.Count;
			
			int j = 0;
			foreach (Touch touch in touchDict.Values)
			{
				FOutID[j] = touch.ID;
				FAge[j] = touch.Frame;
				FOutStartXY[j] = touch.IntitialXY;
				FOutDeltaXY[j] = touch.DeltaXY;
				FOutDistance[j] = touch.Distance;
				FOutLastXY[j] = touch.LastFrameXY;
				FOutIsGone[j] = !touch.Alive;
				FOutIsNew[j] = (touch.Frame == 0);
				FOutTimestamp[j] = touch.Timestamp;
				j++;
			}
			
			//Remove dead IDs from dict for next frame
			for (int i = 0; i < deadKeys.Count; i++)
			{
				touchDict.Remove(deadKeys[i]);
			}
			
		}
	}
}
