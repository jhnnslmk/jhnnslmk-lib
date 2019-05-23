#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using System.Text.RegularExpressions;
using System.Collections.Generic;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "RegExpr", Category = "Advanced", Help = "Basic template with one string in/out", Tags = "c#")]
	#endregion PluginInfo
	public class AdvancedRegExprNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public IDiffSpread<string> FInput;
		
		[Input("Regular Expression")]
		public IDiffSpread<ISpread<string>> FInRegExp;
		
		[Output("Output")]
		public ISpread<ISpread<string>> FOutput;
		
		private List<MatchCollection> matchList = new List<MatchCollection>();
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (FInput.IsChanged || FInRegExp.IsChanged || true)
			{
				FOutput.SliceCount = FInput.SliceCount;
				
				for (int i = 0; i < FInput.SliceCount; i++)
				{
					FLogger.Log(LogType.Debug, "-----------------");
					FLogger.Log(LogType.Debug, "-----------------");
					FLogger.Log(LogType.Debug, "-----------------");
					FLogger.Log(LogType.Debug, "INPUT ["+i.ToString()+"]:" + FInput[i]);
					
					matchList.Clear();
					FOutput[i].SliceCount = 0;
					for (int j = 0; j < FInRegExp[i].SliceCount; j++)
					{
						MatchCollection matches = Regex.Matches(FInput[i], FInRegExp[i][j]);
						matchList.Add(matches);
						
						FLogger.Log(LogType.Debug, FInRegExp[i][j]);
						FOutput[i].SliceCount += matches.Count;
						
						for (int k = 0; k < matches.Count; k++)
						{
							FOutput[i][j] = matches[k].Groups[1].Value;
						}
					}
					
					foreach (MatchCollection m in matchList)
					{
						FLogger.Log(LogType.Debug, m.Count.ToString());
					}
					
				}
			}
		}
	}
	
}
