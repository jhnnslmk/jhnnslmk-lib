using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Phidgets;
using Phidgets.Events;

namespace VVVV.Nodes
{
    class WrapperBridge: Phidgets<Phidgets.Bridge>, IPhidgetsWrapper
    {

        bool FChanged = false;
		bool FBridgeDataChanged = false;

        #region constructor

        public WrapperBridge()
            : base()
        {

        }

        public WrapperBridge(int SerialNumber)
            : base(SerialNumber)
        {

        }

        #endregion constructor


        #region setter fuctions

        public void SetDataRate(int value)
        {
            FPhidget.DataRate = value;
        }


        #endregion setter function


        #region getter functions

        public double GetBridgeData(int Index)
        {
            return FPhidget.bridges[Index].BridgeValue;
        }

        //public int GetOutputCount()
        //{
        //    //return FPhidget.outputs.Count;
        //}

        //public bool GetOutputState(int Index)
        //{
        //    //return FPhidget.outputs[Index];
        //}

        public int GetBridgeCount()
        {
            return FPhidget.bridges.Count;
        }

        //public int GetSensorValue(int Index)
        //{
        //    //return FPhidget.sensors[Index].Value;
        //}

        //public int GetSensorRawValue(int Index)
        //{
        //    return FPhidget.sensors[Index].RawValue;
        //}

        //public int GetDataRate(int Index)
        //{
        //    return FPhidget.sensors[Index].DataRate;
        //}

        //public int GetDataRateMin(int Index)
        //{
        //    return FPhidget.sensors[Index].DataRateMin;
        //}


        //public int GetDataRateMax(int Index)
        //{
        //    return FPhidget.sensors[Index].DataRateMax;
        //}

        //public bool GetRadiometric()
        //{
        //    return FPhidget.ratiometric;
        //}



        #endregion getter functions




        public override void AddChangedHandler()
        {
            //FPhidget.SensorChange += new SensorChangeEventHandler(SensorChange);
            //FPhidget.InputChange += new InputChangeEventHandler(InputChange);
            //FPhidget.OutputChange += new OutputChangeEventHandler(OutputChange);
            FPhidget.BridgeData += new BridgeDataEventHandler(NewBridgeData);
        }

        void OutputChange(object sender, OutputChangeEventArgs e)
        {
            
        }

        void NewBridgeData(object sender, BridgeDataEventArgs e)
        {
            FChanged = true;
            FBridgeDataChanged = true;
        }

        public override void RemoveChangedHandler()
        {
            //FPhidget.SensorChange -= new SensorChangeEventHandler(SensorChange);
            //FPhidget.InputChange -= new InputChangeEventHandler(InputChange);
            //FPhidget.OutputChange -= new OutputChangeEventHandler(OutputChange);
        }


        public int Count
        {
            get { return FPhidget.bridges.Count; }
        }


        public bool Changed
        {
            get
            {
                bool temp = FChanged;
                FChanged = false;
                return temp;
            }
        }

		public bool BridgeDataChanged
		{
			get
			{
				bool temp = FBridgeDataChanged;
                FBridgeDataChanged = false;
				return temp;
			}
		}


    }
}
