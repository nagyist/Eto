using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms
{
	public class RadioButtonHandler : WindowsControl<swf.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		List<RadioButton> group;

		public class EtoRadioButton : swf.RadioButton
		{
			public EtoRadioButton()
			{
				this.SetStyle(swf.ControlStyles.StandardClick | swf.ControlStyles.StandardDoubleClick, true);
			}
		}
		
		public RadioButtonHandler()
		{
			Control = new EtoRadioButton();
			Control.AutoSize = true;
			Control.Click += (sender, e) => Callback.OnClick (Widget, EventArgs.Empty);
			Control.CheckedChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public void Create(RadioButton controller)
		{
			if (controller != null)
			{
				var controllerInner = (RadioButtonHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new List<RadioButton>();
					controllerInner.group.Add(controller);
					controllerInner.Control.Click += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add(Widget);
				Control.Click += controllerInner.control_RadioSwitch;
			}
		}

		void control_RadioSwitch(object sender, EventArgs e)
		{
			if (group != null)
			{
				foreach (RadioButton item in group)
				{
					item.Checked = (item.ControlObject == sender);
				}
			}
		}
		
		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
	}
}
