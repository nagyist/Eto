using System;
using Eto.Forms;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SD = System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace Eto.iOS.Forms
{
	public class ApplicationHandler : WidgetHandler<UIApplication, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;

		public static ApplicationHandler Instance
		{
			get { return Application.Instance.Handler as ApplicationHandler; }
		}

		public string DelegateClassName { get; set; }

		public UIApplicationDelegate AppDelegate { get; private set; }

		public ApplicationHandler()
		{
			DelegateClassName = "EtoAppDelegate";
			UIApplication.CheckForIllegalCrossThreadCalls = false;
			Control = UIApplication.SharedApplication;
		}

		public float StatusBarAdjustment
		{
			get
			{
				if (Platform.IsIos7)
				{
					var statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
					return Math.Min(statusBarFrame.Height, statusBarFrame.Width);
				}
				return 0f;
			}
		}

		public void Run(string[] args)
		{
			if (!attached)
			{
				UIApplication.Main(args, null, DelegateClassName);
			}
			else
			{
				Initialize(Control.Delegate);
			}
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Initialize(UIApplicationDelegate appdelegate)
		{
			AppDelegate = appdelegate;
			
			Callback.OnInitialized(Widget, EventArgs.Empty);
		}

		public void Invoke(Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action();
			else
			{
				UIApplication.SharedApplication.InvokeOnMainThread(delegate
				{
					action(); 
				});
			}
		}

		public void AsyncInvoke(Action action)
		{
			UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate
			{
				action(); 
			});
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			yield break;
		}

		public void Quit()
		{
			//UIApplication.SharedApplication...SharedApplication.Terminate((NSObject)NSApplication.SharedApplication.KeyWindow ?? AppDelegate);
		}

		public bool QuitIsSupported { get { return false; } }

		public void Open(string url)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}

		public Keys CommonModifier
		{
			get { return Keys.Application; }
		}

		public Keys AlternateModifier
		{
			get { return Keys.Alt; }
		}

		public string BadgeLabel
		{
			get { return Control.ApplicationIconBadgeNumber > 0 ? Convert.ToString(Control.ApplicationIconBadgeNumber) : null; }
			set
			{ 
				if (string.IsNullOrEmpty(value))
					Control.ApplicationIconBadgeNumber = 0;
				else
				{
					int result;
					if (Int32.TryParse(value, out result))
						Control.ApplicationIconBadgeNumber = result;
					else
					{
						Debug.WriteLine("iOS: Application.BadgeLabel only supports numeric values");
						Control.ApplicationIconBadgeNumber = 0;
					}
				}
			}
		}


		public void Restart()
		{
			throw new NotImplementedException();
		}

		public void RunIteration()
		{
			throw new NotImplementedException();
		}

		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands)
		{
			throw new NotImplementedException();
		}
	}
}
