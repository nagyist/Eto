using System;
using Eto.Forms;
#if IOS
using MonoTouch.Foundation;
#else
using MonoMac.Foundation;
#endif

#if IOS
namespace Eto.iOS.Forms

#elif OSX
namespace Eto.Mac.Forms
#endif
{
	public class UITimerHandler : WidgetHandler<NSTimer, UITimer, UITimer.ICallback>, UITimer.IHandler
	{
		double interval = 1f;
		
		class Helper
		{
			WeakReference handler;
			public UITimerHandler Handler { get { return (UITimerHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public void Elapsed()
			{
				var h = Handler;
				if (h != null)
				{
					h.Callback.OnElapsed(h.Widget, EventArgs.Empty);
				}
			}
		}

		public void Start ()
		{
			Stop();
			var helper = new Helper { Handler = this };
			Control = NSTimer.CreateRepeatingTimer(interval, helper.Elapsed);
			NSRunLoop.Current.AddTimer(Control, NSRunLoopMode.Default);
		}

		public void Stop ()
		{
			if (Control != null)
			{
				Control.Invalidate();
				Control.Dispose ();
				Control = null;
			}
		}

		public double Interval
		{
			get { return interval; }
			set { interval = value; }
		}
	}
}

