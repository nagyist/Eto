using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;
using Eto.Drawing;

namespace Eto.Mac.Forms
{
	public class ScreenHandler : WidgetHandler<NSScreen, Screen>, Screen.IHandler
	{
		public ScreenHandler (NSScreen screen)
		{
			this.Control = screen;
		}

		static readonly Selector selBackingScaleFactor = new Selector ("backingScaleFactor");

#pragma warning disable 612, 618
		public float RealScale
		{
			get
			{
				if (Control.RespondsToSelector(selBackingScaleFactor))
					return Control.BackingScaleFactor;
				return Control.UserSpaceScaleFactor;
			}
		}
#pragma warning restore 612, 618

		public float Scale
		{
			get { return 1f; }
		}

		public RectangleF Bounds
		{
			get
			{ 
				var bounds = Control.Frame;
				var origin = NSScreen.Screens[0].Frame.Bottom;
				bounds.Y = origin - bounds.Height - bounds.Y;
				return bounds.ToEto();

			}
		}

		public RectangleF WorkingArea
		{
			get
			{ 
				var workingArea = Control.VisibleFrame;
				var origin = NSScreen.Screens[0].Frame.Bottom;
				workingArea.Y = origin - workingArea.Height - workingArea.Y;
				return workingArea.ToEto();
			}
		}

		public int BitsPerPixel
		{
			get { return NSGraphics.BitsPerPixelFromDepth (Control.Depth); }
		}

		public bool IsPrimary
		{
			get { return Control == NSScreen.Screens[0]; }
		}
	}
}

