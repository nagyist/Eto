using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : Pen.IHandler
	{
		class PenControl {
			float[] cgdashes;
			DashStyle dashStyle;
			CGLineCap lineCap;
			float thickness;
			float cgoffset;

			public CGColor Color { get; set; }
			
			public float Thickness
			{
				get { return thickness; }
				set {
					thickness = value;
					SetDashStyle ();
				}
			}
			
			public CGLineJoin LineJoin { get; set; }
			
			public CGLineCap LineCap
			{
				get { return lineCap; }
				set {
					lineCap = value;
					SetDashStyle ();
				}
			}
			
			public float MiterLimit { get; set; }
			
			public DashStyle DashStyle
			{
				get { return dashStyle; }
				set {
					dashStyle = value;
					SetDashStyle ();
				}
			}
			
			void SetDashStyle ()
			{
				if (DashStyle == null || DashStyle.IsSolid) {
					cgdashes = null;
				} else {
					// TODO: this is not quite perfect for Square/Round for small thicknesses
					
					var dashes = DashStyle.Dashes;
					cgoffset = DashStyle.Offset * Thickness;
					
					if (LineCap == CGLineCap.Butt)
						cgdashes = Array.ConvertAll (dashes, x => x * Thickness);
					else {
						if (Math.Abs(Thickness - 1) < 0.01f)
							cgoffset += Thickness / 2;
						cgdashes = new float[dashes.Length];
						for (int i = 0; i < cgdashes.Length; i++) {
							var dash = dashes [i] * Thickness;
							if ((i % 2) == 1) {
								// gap must include square/round thickness
								dash += Thickness;
							} else {
								// dash must exclude square/round thickness
								dash -= Thickness;
							}
							cgdashes [i] = dash;
						}
					}
				}
			}

			public void Apply (GraphicsHandler graphics)
			{
				graphics.Control.SetStrokeColor (Color);
				graphics.Control.SetLineCap (LineCap);
				graphics.Control.SetLineJoin (LineJoin);
				graphics.Control.SetLineWidth (Thickness);
				graphics.Control.SetMiterLimit (MiterLimit);
				if (cgdashes != null)
					graphics.Control.SetLineDash (cgoffset, cgdashes);
			}
		}

		public object Create (Color color, float thickness)
		{
			return new PenControl {
				Color = color.ToCGColor (),
				Thickness = thickness,
				MiterLimit = 10f,
				LineCap = PenLineCap.Square.ToCG ()
			};
		}

		public Color GetColor (Pen widget)
		{
			return ((PenControl)widget.ControlObject).Color.ToEtoColor ();
		}

		public void SetColor (Pen widget, Color color)
		{
			((PenControl)widget.ControlObject).Color = color.ToCGColor ();
		}

		public float GetThickness (Pen widget)
		{
			return ((PenControl)widget.ControlObject).Thickness;
		}

		public void SetThickness (Pen widget, float thickness)
		{
			((PenControl)widget.ControlObject).Thickness = thickness;
		}

		public PenLineJoin GetLineJoin (Pen widget)
		{
			return ((PenControl)widget.ControlObject).LineJoin.ToEto ();
		}

		public void SetLineJoin (Pen widget, PenLineJoin lineJoin)
		{
			((PenControl)widget.ControlObject).LineJoin = lineJoin.ToCG ();
		}

		public PenLineCap GetLineCap (Pen widget)
		{
			return ((PenControl)widget.ControlObject).LineCap.ToEto ();
		}

		public void SetLineCap (Pen widget, PenLineCap lineCap)
		{
			((PenControl)widget.ControlObject).LineCap = lineCap.ToCG ();
		}

		public float GetMiterLimit (Pen widget)
		{
			return ((PenControl)widget.ControlObject).MiterLimit;
		}

		public void SetMiterLimit (Pen widget, float miterLimit)
		{
			((PenControl)widget.ControlObject).MiterLimit = miterLimit;
		}

		public void SetDashStyle (Pen widget, DashStyle dashStyle)
		{
			((PenControl)widget.ControlObject).DashStyle = dashStyle;
		}

		public void Apply (Pen widget, GraphicsHandler graphics)
		{
			((PenControl)widget.ControlObject).Apply (graphics);
		}
	}
}

