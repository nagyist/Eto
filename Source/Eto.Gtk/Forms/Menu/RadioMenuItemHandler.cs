using System;
using Eto.Forms;

namespace Eto.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class RadioMenuItemHandler : MenuActionItemHandler<Gtk.RadioMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		string text;
		Keys shortcut;
		Gtk.AccelLabel label;
		Gtk.Label accelLabel;
		bool isActivating;
		RadioMenuItemHandler controller;

		public void Create(RadioMenuItem controller)
		{
			if (controller != null)
			{
				Control = new Gtk.RadioMenuItem((Gtk.RadioMenuItem)controller.ControlObject);
				this.controller = (RadioMenuItemHandler)controller.Handler;
			}
			else
			{
				this.controller = this;
				Control = new Gtk.RadioMenuItem(string.Empty);
				foreach (Gtk.Widget w in Control.Children)
				{
					Control.Remove(w);
				}
			}

			var hbox = new Gtk.HBox(false, 4);
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			hbox.Add(label);
			accelLabel = new Gtk.Label();
			accelLabel.Xalign = 1;
			accelLabel.Visible = false;
			hbox.Add(accelLabel);
			Control.Add(hbox);
			Control.Toggled += Connector.HandleToggled;
		}

		protected new RadioMenuItemConnector Connector { get { return (RadioMenuItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new RadioMenuItemConnector();
		}

		protected class RadioMenuItemConnector : WeakConnector
		{
			public new RadioMenuItemHandler Handler { get { return (RadioMenuItemHandler)base.Handler; } }

			public void HandleToggled(object sender, EventArgs e)
			{
				var handler = Handler;
				if (!handler.controller.isActivating)
				{
					handler.Callback.OnClick(handler.Widget, e);
				}
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				label.TextWithMnemonic = text;
			}
		}

		public string ToolTip
		{
			get { return label.TooltipText; }
			set { label.TooltipText = value; }
		}

		public Keys Shortcut
		{
			get { return shortcut; }
			set
			{
				shortcut = value;
				accelLabel.Text = value.ToShortcutString();
				accelLabel.Visible = accelLabel.Text.Length > 0;
			}
		}

		public bool Checked
		{
			get { return Control.Active; }
			set
			{ 
				controller.isActivating = true;
				Control.Active = value;
				controller.isActivating = false;
			}
		}

		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}

		public void AddMenu(int index, MenuItem item)
		{
			if (Control.Submenu == null)
				Control.Submenu = new Gtk.Menu();
			((Gtk.Menu)Control.Submenu).Insert((Gtk.Widget)item.ControlObject, index);
		}

		public void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null)
				return;
			var menu = (Gtk.Menu)Control.Submenu;
			menu.Remove((Gtk.Widget)item.ControlObject);
			if (menu.Children.Length == 0)
			{
				Control.Submenu = null;
			}
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}
	}
}
