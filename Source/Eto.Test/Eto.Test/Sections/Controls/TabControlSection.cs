using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class TabControlSection : Panel
	{
		TabControl tabControl;

		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			Content = Create();
		}

		public virtual Control Create()
		{
			return new TableLayout(
				new TableLayout(new TableRow(null, AddTab(), RemoveTab(), SelectTab(), null)),
				tabControl = DefaultTabs()
			);
		}

		Control AddTab()
		{
			var control = new Button { Text = "Add Tab" };
			control.Click += (s, e) =>
			{
				var tab = new TabPage
				{ 
					Text = "Tab " + (tabControl.Pages.Count + 1),
					Content = tabControl.Pages.Count % 2 == 0 ? TabOne() : TabTwo()
				};
				tabControl.Pages.Add(tab);
			};
			return control;
		}

		Control RemoveTab()
		{
			var control = new Button { Text = "Remove Tab" };
			control.Click += (s, e) =>
			{
				if (tabControl.SelectedIndex >= 0 && tabControl.Pages.Count > 0)
				{
					tabControl.Pages.RemoveAt(tabControl.SelectedIndex);
				}
			};
			return control;
		}

		Control SelectTab()
		{
			var control = new Button { Text = "Select Tab" };
			var rnd = new Random();
			control.Click += (s, e) =>
			{
				if (tabControl.Pages.Count > 0)
				{
					tabControl.SelectedIndex = rnd.Next(tabControl.Pages.Count);
				}
			};
			return control;
		}

		TabControl DefaultTabs()
		{
			var control = CreateTabControl();
			LogEvents(control);

			control.Pages.Add(new TabPage { Text = "Tab 1", Content = TabOne() });

			control.Pages.Add(new TabPage
			{ 
				Text = "Tab 2",
				Image = TestIcons.TestIcon,
				Content = TabTwo()
			});

			control.Pages.Add(new TabPage { Text = "Tab 3" });

			foreach (var page in control.Pages)
				LogEvents(page);

			return control;
			
		}

		protected virtual TabControl CreateTabControl()
		{
			return new TabControl();
		}

		Control TabOne()
		{
			var control = new Panel();
			
			control.Content = new LabelSection();
			
			return control;
		}

		Control TabTwo()
		{
			var control = new Panel();
			
			control.Content = new TextAreaSection { Border = BorderType.None };
			
			return control;
		}

		void LogEvents(TabControl control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);	
			};
		}

		void LogEvents(TabPage control)
		{
			control.Click += delegate
			{
				Log.Write(control, "Click, Item: {0}", control.Text);
			};
		}

		/// <summary>
		/// Gets the platform with a themed tab control.
		/// </summary>
		/// <remarks>
		/// Clone the current generator and add themed handlers for TabControl and TabPage.
		/// </remarks>
		public static Platform ThemedPlatform
		{
			get
			{
				var platform = (Platform)Activator.CreateInstance(Platform.Instance.GetType());

				platform.Add<TabControl.IHandler>(() => new Eto.Test.Handlers.TabControlHandler());
				platform.Add<TabPage.IHandler>(() => new Eto.Test.Handlers.TabPageHandler());

				return platform;
			}
		}
	}
}

