using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.AppKit;
using Eto.Drawing;
using Eto.Mac.Drawing;
using MonoMac.Foundation;

namespace Eto.Mac.Forms.Controls
{
	public class NumericUpDownHandler : MacView<NSView, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		readonly NSTextField text;
		readonly NSStepper stepper;
		Font font;
		Size? naturalSize;

		public override NSView ContainerControl { get { return Control; } }

		public class EtoTextField : NSTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }
		}

		class EtoNumericUpDownView : MacEventView
		{
			public override void SetFrameSize(System.Drawing.SizeF newSize)
			{
				base.SetFrameSize(newSize);
				var views = Subviews;
				var text = views[0];
				var splitter = views[1];
				var offset = (newSize.Height - text.Frame.Height) / 2;
				text.SetFrameOrigin(new SD.PointF(0, offset));
				text.SetFrameSize(new SD.SizeF(newSize.Width - splitter.Frame.Width, text.Frame.Height));
				offset = (newSize.Height - splitter.Frame.Height) / 2;
				splitter.SetFrameOrigin(new SD.PointF(newSize.Width - splitter.Frame.Width, offset));
			}
		}

		class EtoStepper : NSStepper
		{
			public override bool AcceptsFirstResponder()
			{
				return false;
			}
		}

		public override object EventObject
		{
			get { return text; }
		}

		public NumericUpDownHandler()
		{
			this.Control = new EtoNumericUpDownView
			{
				Handler = this,
				AutoresizesSubviews = false
			};
			text = new EtoTextField
			{
				WeakHandler = new WeakReference(this),
				Bezeled = true,
				Editable = true
			};
			text.Changed += HandleTextChanged;
			
			stepper = new EtoStepper();
			stepper.Activated += HandleStepperActivated;
			MinValue = 0;
			MaxValue = 100;
			Value = 0;
			Control.AddSubview(text);
			Control.AddSubview(stepper);
		}

		static void HandleStepperActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)sender).Superview) as NumericUpDownHandler;
			if (handler != null)
			{
				var val = handler.stepper.DoubleValue;
				if (Math.Abs(val) < 1E-10)
				{
					handler.text.IntValue = 0;
				}
				else
				{
					handler.text.DoubleValue = handler.stepper.DoubleValue;
				}
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		static void HandleTextChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(((NSView)((NSNotification)sender).Object).Superview) as NumericUpDownHandler;
			if (handler != null)
			{
				handler.stepper.DoubleValue = handler.text.DoubleValue;
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			var size = GetNaturalSize(Size.MaxValue);
			Control.Frame = new System.Drawing.RectangleF(0, 0, size.Width, size.Height);
			HandleEvent(Eto.Forms.Control.KeyDownEvent);
		}

		public override void PostKeyDown(KeyEventArgs e)
		{
			base.PostKeyDown(e);
			if (e.KeyData == Keys.Down)
			{
				var val = Value;
				var newval = Math.Max(val - 1, MinValue);
				if (newval < val)
				{
					Value = newval;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
				e.Handled = true;
			}
			else if (e.KeyData == Keys.Up)
			{
				var val = Value;
				var newval = Math.Min(val + 1, MaxValue);
				if (newval > val)
				{
					Value = newval;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
				e.Handled = true;
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (naturalSize == null)
			{
				text.SizeToFit();
				stepper.SizeToFit();
				var naturalHeight = Math.Max(text.Frame.Height, stepper.Frame.Height);
				naturalSize = new Size(80, (int)naturalHeight);
			}
			return naturalSize.Value;
		}

		public bool ReadOnly
		{
			get { return text.Enabled; }
			set
			{ 
				text.Enabled = value;
				stepper.Enabled = value;
			}
		}

		public double Value
		{
			get { return text.DoubleValue; }
			set
			{
				if (Math.Abs(value) < 1E-10)
				{
					stepper.IntValue = text.IntValue = 0;
				}
				else
				{
					stepper.DoubleValue = text.DoubleValue = value;
				}
			}
		}

		public double MinValue
		{
			get
			{
				return stepper.MinValue;
			}
			set
			{
				stepper.MinValue = value;
			}
		}

		public double MaxValue
		{
			get
			{
				return stepper.MaxValue;
			}
			set
			{
				stepper.MaxValue = value;
			}
		}

		public override bool Enabled
		{
			get
			{
				return stepper.Enabled;
			}
			set
			{
				stepper.Enabled = value;
				text.Enabled = value;
			}
		}

		public Font Font
		{
			get
			{
				return font ?? (font = new Font(new FontHandler(text.Font)));
			}
			set
			{
				font = value;
				text.Font = font == null ? null : font.ControlObject as NSFont;
				text.SizeToFit();
				LayoutIfNeeded();
			}
		}
	}
}
