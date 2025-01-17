using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="Button"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : AndroidCommonControl<aw.Button, Button, Button.ICallback>, Button.IHandler
	{
		public ButtonHandler()
		{
			Control = new aw.Button(aa.Application.Context);
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		public Eto.Drawing.Image Image
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ButtonImagePosition ImagePosition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}