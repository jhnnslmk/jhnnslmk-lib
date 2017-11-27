#region usings
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes.UI
{
	
	public class ResolutionManager
	{
		private static ResolutionManager _instance;
		private Dictionary<string, Vector2D> _resDict;
		private ResolutionManager()
		{
			_resDict = new Dictionary<string, Vector2D>();
			//ResolutionManager.RegisterResolution("vvvv", new Vector2D(2, 2)); <-------------------------
		}
		
		public static ResolutionManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new ResolutionManager();
				ResolutionManager.RegisterResolution("vvvv", new Vector2D(2, 2));
			}
			return _instance;
		}
		
		public static void RegisterResolution (string name, Vector2D res)
		{
			ResolutionManager mng = ResolutionManager.GetInstance();
			mng._resDict.Add(name, res);
			ResolutionManager.UpdateEnumeration();
		}
		
		private static void UpdateEnumeration()
		{
			ResolutionManager mng = ResolutionManager.GetInstance();
			string[] keyArray = mng._resDict.Keys.ToArray();
			EnumManager.UpdateEnum("ResolutionManager", "vvvv", keyArray);
		}
		
		public static Vector2D GetResolution(string name)
		{
			ResolutionManager mng = ResolutionManager.GetInstance();
			return mng._resDict[name];
		}
		
		public static Vector2D GetSize(string name)
		{
			ResolutionManager mng = ResolutionManager.GetInstance();
			return new Vector2D(2, 2) / mng._resDict[name];
		}
	}
	
	#region PluginInfo
	[PluginInfo(Name = "ResolutionManager", Category = "UI", Help = "Basic template with one value in/out", Tags = "c#", AutoEvaluate = true)]
	#endregion PluginInfo
	public class ResolutionManagerNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Resolution", DefaultValues = new double[]{1,1}, Order = 0)]
		public IDiffSpread<Vector2D> FInResolution;
		
		[Input("Name", Order = 1)]
		public IDiffSpread<string> FInName;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (FInResolution.IsChanged || FInName.IsChanged)
			{
				for (int i = 0; i < SpreadMax; i++)
				{
					ResolutionManager.RegisterResolution(FInName[i], FInResolution[i]);
				}
			}
			
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
