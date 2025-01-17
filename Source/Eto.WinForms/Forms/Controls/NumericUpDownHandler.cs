using System;
using SD = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms
{
	public class NumericUpDownHandler : WindowsControl<swf.NumericUpDown, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new swf.NumericUpDown
			{
				Maximum = 100,
				Minimum = 0,
				Width = 80
			};
			Control.ValueChanged += delegate
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			LeakHelper.UnhookObject(Control);
		}

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public double Value
		{
			get { return (double)Control.Value; }
			set { Control.Value = (decimal)value; }
		}

		public double MinValue
		{
			get { return (double)Control.Minimum; }
			set { Control.Minimum = (decimal)value; }
		}

		public double MaxValue
		{
			get { return (double)Control.Maximum; }
			set { Control.Maximum = (decimal)value; }
		}
	}
}
