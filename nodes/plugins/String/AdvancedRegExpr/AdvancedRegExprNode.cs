#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using System.Text.RegularExpressions;

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
		public IDiffSpread<string> FInRegExp;
		
		[Output("Output")]
		public ISpread<ISpread<string>> FOutput;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (FInput.IsChanged || FInRegExp.IsChanged)
			{
				
				
				FOutput.SliceCount = SpreadMax;
				
				for (int i = 0; i < SpreadMax; i++)
				{
					MatchCollection matches = Regex.Matches(FInput[i], FInRegExp[i]);
					FOutput[i].SliceCount = matches.Count;
					
					for (int j = 0; j < matches.Count; j++)
					{
						FOutput[i][j] = matches[j].Groups[1].Value;
					}
				}
			}
		}
	}
}
