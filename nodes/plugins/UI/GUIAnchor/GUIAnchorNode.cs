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
	Category = "GUI",
	Help = "Template with some gui elements",
	Tags = "c#",
	InitialWindowWidth = 233,
	InitialWindowHeight = 130,
	AutoEvaluate = true)]
	#endregion PluginInfo
	public class GUIAnchorNode : UserControl, IPluginEvaluate
	{
		#region fields & pins
		
		[Input("Canvas")]
		public ISpread<UI.Canvas> FInCanvas;
		
		[Input("Size", DefaultValues = new double[]{1,1})]
		public ISpread<Vector2D> FInSize;
		
		[Input("Offset")]
		public ISpread<Vector2D> FInOffset;
		
		[Input("Resolution", EnumName = "ResolutionManager")]
        public IDiffSpread<EnumEntry> FInResolution;
		
		[Output("Transform")]
		public ISpread<Matrix4x4> FOutTransform;
		
		[Output("Canvas")]
		public ISpread<UI.Canvas> FOutCanvas;
		
		[Config("AlignmentConfig", Visibility = PinVisibility.Hidden)]
        public ISpread<string> FConfAlignment;
		
		private const int _defaultAlignment = 0;
		private Vector2D _parent = UI.AlignmentVectors[_defaultAlignment];
		private Vector2D _element = UI.AlignmentVectors[_defaultAlignment];
		private int _parentInt = _defaultAlignment;
		private int _elementInt = _defaultAlignment;
		
		[Import()]
		public ILogger FLogger;
		
		#endregion fields & pins
		
		#region constructor and init
		
		public GUIAnchorNode()
		{
			//setup the gui
			InitializeComponent();
		}
		
		private enum Alignment
		{
			LeftTop, CenterTop, RightTop,
			LeftCenter, CenterCenter, RightCenter,
			LeftBottom, CenterBottom, RightBottom
		}
		
		void InitializeComponent()
		{
			//clear controls in case init is called multiple times
			Controls.Clear();
			
			FlowLayoutPanel p = new FlowLayoutPanel();
			p.FlowDirection = FlowDirection.LeftToRight;
			
			p.AutoSize = true;
			p.Controls.Add(CreateGrid("Parent"));
			p.Controls.Add(CreateGrid("Element"));
			
			Controls.Add(p);
		}
		
		private void AllCheckBoxes_CheckedChanged(Object sender, EventArgs e) {
			// Check of the raiser of the event is a checked Checkbox.
			// Of course we also need to to cast it first.
			if (((RadioButton)sender).Checked) {
				// This is the correct control.
				RadioButton rb = (RadioButton)sender;
				if (rb.Parent.Parent.Text == "Parent")
				{
					_parent = UI.AlignmentVectors[(int)rb.Tag];
					_parentInt = (int)rb.Tag;
				}
				
				if (rb.Parent.Parent.Text == "Element")
				{
					_element = UI.AlignmentVectors[(int)rb.Tag];
					_elementInt = (int)rb.Tag;
				}
				
				FConfAlignment[0] = _parent.ToString() + ", " +_element.ToString();
				FLogger.Log(LogType.Debug, rb.Parent.Parent.Text + " " + rb.Tag.ToString());
			}
		}
		
		Control CreateGrid(string title)
		{
			GroupBox group = new GroupBox();
			group.Text = title;
			//			group.AutoSize = true;
			//			group.Dock = DockStyle.Fill;
			//			group.Anchor = AnchorStyles.None;
			group.Size = new Size(110, 120);
			
			float size = 1.0f/3.0f;
			TableLayoutPanel tp = new TableLayoutPanel();
			tp.RowCount = 3;
			tp.ColumnCount = 3;
			tp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tp.Size = new Size(100, 100);
			tp.Location = new System.Drawing.Point(5, 15);
			
			for (int i = 0; i < 3; i++)
			{
				tp.RowStyles.Add(new RowStyle(SizeType.Percent, size));
				tp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, size));
			}
			
			for (int i = 0; i < 9; i++)
			{
				var radio = new RadioButton();
				if (i == _defaultAlignment) radio.Checked = true;
				radio.Tag = (Alignment)i;
				radio.Anchor = AnchorStyles.None;
				radio.AutoSize = true;
				radio.Dock = DockStyle.None;
				radio.CheckedChanged += AllCheckBoxes_CheckedChanged;
				tp.Controls.Add(radio);
			}
			
			group.Controls.Add(tp);
			
			return group;
		}
		
		#endregion constructor and init
		
		public void Evaluate(int SpreadMax)
		{
			FOutTransform.SliceCount = SpreadMax;
			FOutCanvas.SliceCount = SpreadMax;
			
			for (int i = 0; i < SpreadMax; i++)
			{
				UI.Canvas c = FInCanvas[i];
				if (c == null) c = new UI.Canvas(2 * VMath.IdentityMatrix);
				Matrix4x4 m;
				
				m = VMath.Translate(new Vector3D(-1*_element*0.5f, 0));
				
				m *= VMath.Scale(new Vector3D(ResolutionManager.GetSize(FInResolution[0].Name) * FInSize[i], 1));
//				FLogger.Log(LogType.Debug, FInResolution[0].Name);
				
				m *= VMath.Translate(c.Center);
				
				m *= VMath.Translate(new Vector3D((_parent*0.5f*c.Size-_element*ResolutionManager.GetSize(FInResolution[0].Name) * FInOffset[i]), 0));
				
				
				FOutTransform[i] = m;
				FOutCanvas[i] = new UI.Canvas(m);
				
			}
		}
	}
}
