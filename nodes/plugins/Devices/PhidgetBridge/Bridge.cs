#region usings
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using Phidgets;

#endregion usings

namespace VVVV.Nodes
{
    #region PluginInfo
    [PluginInfo(Name = "Bridge",
                Category = "Devices",
                Version = "Phidget",
                Help = "Controls the Phidget Bridge Kits",
                Tags = "controller, Bridge",
                Author = "readme",
                AutoEvaluate = true
)]
    #endregion
    public class Bridge : IPluginEvaluate
    {
        #region fields & pins
        [Input("DataRate (ms)", DefaultValue = 16)]
        IDiffSpread<int> FDataRateIn;

        [Input("Serial", DefaultValue = 0, IsSingle = true, AsInt = true, MinValue = 0, MaxValue = int.MaxValue)]
        IDiffSpread<int> FSerial;

        //Output
        [Output("Attached")]
        ISpread<bool> FAttached;

        [Output("Bridges")]
        ISpread<double> FBridgesOut;

        [Output("Radiometric", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FRadiometricOut;

        [Output("DataRate(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateOut;

        [Output("DataRateMin(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateMinOut;

        [Output("DataRateMax(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateMaxOut;

        //Logger
        [Import()]
        ILogger FLogger;


        //private Fields
        WrapperBridge FBridge;
        private bool disposed;
        private bool FInit = true;

        #endregion fields & pins

        //called when data for any output pin is requested
        public void Evaluate(int SpreadMax)
        {
            try
            {
                if (FSerial.IsChanged)
                {
                    if (FBridge != null)
                    {
                        FBridge.Close();
                        FBridge = null;
                    }
                    FBridge = new WrapperBridge(FSerial[0]);
                    FInit = false;
                }

                if (FBridge.Attached && FInit == false)
                {

                    if (!(FBridge.FPhidget.ID.ToString() == Phidget.PhidgetID.LINEAR_TOUCH.ToString() || FBridge.FPhidget.ID.ToString() == Phidget.PhidgetID.ROTARY_TOUCH.ToString()))
                    {

                        if (FDataRateIn.IsChanged || FInit)
                        {
                            for (int i = 0; i < FBridge.GetBridgeCount(); i++)
                            {
                                FBridge.SetDataRate(FDataRateIn[i]);
                            }

                        }
                    }

                    if (FBridge.BridgeDataChanged)
                    {
                        FBridgesOut.SliceCount = FBridge.GetBridgeCount();

                        for (int i = 0; i < FBridge.GetBridgeCount(); i++)
                        {
                            FBridgesOut[i] = Convert.ToDouble(FBridge.GetBridgeData(i));
                        }
                    }

                }
                else
                {
                    FBridgesOut.SliceCount = 0;
                }

                FAttached[0] = FBridge.Attached;

                List<PhidgetException> Exceptions = FBridge.Errors;
                if (Exceptions != null)
                {
                    foreach (Exception e in Exceptions)
                        FLogger.Log(e);
                }
            }
            catch (PhidgetException ex)
            {
                FLogger.Log(ex);
            }
        }
    }
}
