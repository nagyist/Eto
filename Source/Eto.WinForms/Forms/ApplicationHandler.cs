using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Collections.Generic;

namespace Eto.WinForms
{
	public class ApplicationHandler : WidgetHandler<object, Application, Application.ICallback>, Application.IHandler
	{
		string badgeLabel;
		bool attached;
		readonly Thread mainThread;
		SynchronizationContext context;
		public static bool EnableScrollingUnderMouse = true;
		public static bool BubbleMouseEvents = true;
		public static bool BubbleKeyEvents = true;

		public ApplicationHandler()
		{
			mainThread = Thread.CurrentThread;
		}

		public void RunIteration()
		{
			swf.Application.DoEvents();
		}

		public void Restart()
		{
			swf.Application.Restart();
		}

		public string BadgeLabel
		{
			get { return badgeLabel; }
			set
			{
				badgeLabel = value;
				if (TaskbarManager.IsPlatformSupported)
				{
					if (!string.IsNullOrEmpty(badgeLabel))
					{
						var bmp = new sd.Bitmap(16, 16, sd.Imaging.PixelFormat.Format32bppArgb);
						using (var graphics = sd.Graphics.FromImage(bmp))
						{
							DrawBadgeLabel(bmp, graphics);
						}
						var icon = sd.Icon.FromHandle(bmp.GetHicon());

						TaskbarManager.Instance.SetOverlayIcon(icon, badgeLabel);
					}
					else
						TaskbarManager.Instance.SetOverlayIcon(null, null);
				}
			}
		}

		protected virtual void DrawBadgeLabel(sd.Bitmap bmp, sd.Graphics graphics)
		{
			var font = new sd.Font(sd.FontFamily.GenericSansSerif, 9, sd.FontStyle.Bold, sd.GraphicsUnit.Pixel);

			var size = graphics.MeasureString(badgeLabel, font, bmp.Size, sd.StringFormat.GenericTypographic);
			graphics.SmoothingMode = sd.Drawing2D.SmoothingMode.AntiAlias;
			graphics.FillEllipse(sd.Brushes.Red, new sd.Rectangle(0, 0, 16, 16));
			graphics.DrawEllipse(new sd.Pen(sd.Brushes.White, 2), new sd.Rectangle(0, 0, 15, 15));
			var pt = new sd.PointF((bmp.Width - size.Width - 0.5F) / 2, (bmp.Height - size.Height - 1) / 2);
			graphics.DrawString(badgeLabel, font, sd.Brushes.White, pt, sd.StringFormat.GenericTypographic);
		}

		public void Run(string[] args)
		{
			if (!attached)
			{
				swf.Application.EnableVisualStyles();
				if (!EtoEnvironment.Platform.IsMono)
					swf.Application.DoEvents();

				SetOptions();
				// creates sync context
				using (var ctl = new swf.Control())
				{
				}
				context = SynchronizationContext.Current;

				Callback.OnInitialized(Widget, EventArgs.Empty);

				if (Widget.MainForm != null && Widget.MainForm.Loaded)
					swf.Application.Run((swf.Form)Widget.MainForm.ControlObject);
				else
					swf.Application.Run();
			}
			else
			{
				Callback.OnInitialized(Widget, EventArgs.Empty);
			}
		}

		static void SetOptions()
		{
			if (EnableScrollingUnderMouse)
				swf.Application.AddMessageFilter(new ScrollMessageFilter());

			if (BubbleMouseEvents)
			{
				var bubble = new BubbleEventFilter();
				bubble.AddBubbleMouseEvent((c, cb, e) => cb.OnMouseWheel(c, e), null, Win32.WM.MOUSEWHEEL);
				bubble.AddBubbleMouseEvent((c, cb, e) => cb.OnMouseMove(c, e), null, Win32.WM.MOUSEMOVE);
				bubble.AddBubbleMouseEvents((c, cb, e) => cb.OnMouseDown(c, e), true, Win32.WM.LBUTTONDOWN, Win32.WM.RBUTTONDOWN, Win32.WM.MBUTTONDOWN);
				bubble.AddBubbleMouseEvents((c, cb, e) => {
					cb.OnMouseDoubleClick(c, e);
					if (!e.Handled)
						cb.OnMouseDown(c, e);
				}, null, Win32.WM.LBUTTONDBLCLK, Win32.WM.RBUTTONDBLCLK, Win32.WM.MBUTTONDBLCLK);
				bubble.AddBubbleMouseEvent((c, cb, e) => cb.OnMouseUp(c, e), false, Win32.WM.LBUTTONUP, b => MouseButtons.Primary);
				bubble.AddBubbleMouseEvent((c, cb, e) => cb.OnMouseUp(c, e), false, Win32.WM.RBUTTONUP, b => MouseButtons.Alternate);
				bubble.AddBubbleMouseEvent((c, cb, e) => cb.OnMouseUp(c, e), false, Win32.WM.MBUTTONUP, b => MouseButtons.Middle);
				swf.Application.AddMessageFilter(bubble);
			}
			if (BubbleKeyEvents)
			{
				var bubble = new BubbleEventFilter();
				bubble.AddBubbleKeyEvent((c, cb, e) => cb.OnKeyDown(c, e), Win32.WM.KEYDOWN, KeyEventType.KeyDown);
				bubble.AddBubbleKeyEvent((c, cb, e) => cb.OnKeyDown(c, e), Win32.WM.SYSKEYDOWN, KeyEventType.KeyDown);
				bubble.AddBubbleKeyCharEvent((c, cb, e) => cb.OnKeyDown(c, e), Win32.WM.CHAR, KeyEventType.KeyDown);
				bubble.AddBubbleKeyCharEvent((c, cb, e) => cb.OnKeyDown(c, e), Win32.WM.SYSCHAR, KeyEventType.KeyDown);
				bubble.AddBubbleKeyEvent((c, cb, e) => cb.OnKeyUp(c, e), Win32.WM.KEYUP, KeyEventType.KeyUp);
				bubble.AddBubbleKeyEvent((c, cb, e) => cb.OnKeyUp(c, e), Win32.WM.SYSKEYUP, KeyEventType.KeyUp);
				swf.Application.AddMessageFilter(bubble);
			}
		}

		public void Attach(object context)
		{
			attached = true;
			SetOptions();
		}

		public void OnMainFormChanged()
		{
		}

		public void Quit()
		{
			swf.Application.Exit();
		}

		public bool QuitIsSupported { get { return true; } }

		public void Open(string url)
		{
			var info = new ProcessStartInfo(url);
			Process.Start(info);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
					// handled by WindowHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void Invoke(Action action)
		{
			if (Thread.CurrentThread == mainThread)
				action();
			else if (context != null)
				context.Send(state => action(), null);
		}

		public void AsyncInvoke(Action action)
		{
			if (context != null)
				context.Post(state => action(), null);
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			yield break;
		}

		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands)
		{
		}

		public Keys CommonModifier
		{
			get
			{
				return Keys.Control;
			}
		}

		public Keys AlternateModifier
		{
			get
			{
				return Keys.Alt;
			}
		}
	}
}
