using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class SliderHandler : MacControl<NSSlider, Slider, Slider.ICallback>, Slider.IHandler
	{
		SliderOrientation orientation;

		public class EtoSlider : NSSlider, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public SliderHandler()
		{
			Control = new EtoSlider { Handler = this };
			Control.Activated += HandleActivated;
			MinValue = 0;
			MaxValue = 100;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as SliderHandler;
			if (handler != null)
			{
				var newval = (int)Math.Round(handler.Control.DoubleValue);
				if (newval != handler.Control.IntValue)
					handler.Control.IntValue = newval;

				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return Orientation == SliderOrientation.Horizontal ? new Size(80, 30) : new Size(30, 80);
		}

		public int MaxValue
		{
			get { return (int)Control.MaxValue; }
			set
			{ 
				var old = TickFrequency;
				Control.MaxValue = value;
				TickFrequency = old;
			}
		}

		public int MinValue
		{
			get { return (int)Control.MinValue; }
			set
			{
				var old = TickFrequency;
				Control.MinValue = value;
				TickFrequency = old;
			}
		}

		public int Value
		{
			get { return Control.IntValue; }
			set { Control.IntValue = value; }
		}

		public bool SnapToTick
		{
			get { return Control.AllowsTickMarkValuesOnly; }
			set { Control.AllowsTickMarkValuesOnly = value; }
		}

		public int TickFrequency
		{
			get
			{ 
				if (Control.TickMarksCount > 1)
					return ((MaxValue - MinValue) / (Control.TickMarksCount - 1));
				return MaxValue - MinValue;
			}
			set
			{ 
				Control.TickMarksCount = ((MaxValue - MinValue) / value) + 1;
			}
		}

		public SliderOrientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				// wha?!?! no way to do this other than change size or sumthun?
				var size = Control.Frame.Size;
				if (value == SliderOrientation.Vertical)
				{
					size.Height = size.Width + 1;
				}
				else
					size.Width = size.Height + 1;
				Control.SetFrameSize(size);
				LayoutIfNeeded();
			}
		}
	}
}

