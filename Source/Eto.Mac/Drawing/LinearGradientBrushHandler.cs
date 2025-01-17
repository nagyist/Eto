using Eto.Drawing;
using sd = System.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Mac.Drawing
#else
using MonoTouch.CoreGraphics;
using MonoTouch.ImageIO;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="LinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		class BrushObject
		{
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			CGAffineTransform viewTransform = CGAffineTransform.MakeIdentity();
			readonly float[] alpha = { 1f };
			CGPattern pattern;
			GradientWrapMode wrap;
			sd.SizeF tileSize;
			sd.SizeF sectionSize;

			public CGGradient InverseGradient { get; set; }

			public CGGradient Gradient { get; set; }

			public sd.PointF StartPoint { get; set; }

			public sd.PointF EndPoint { get; set; }

			public GradientWrapMode Wrap
			{
				get { return wrap; }
				set
				{
					wrap = value;
					pattern = null;
				}
			}

			public void Apply(GraphicsHandler graphics)
			{
				graphics.SetFillColorSpace();

				#if OSX
				if (graphics.DisplayView != null)
				{
					// adjust for position of the current view relative to the window
					var pos = graphics.DisplayView.ConvertPointToView(sd.PointF.Empty, null);
					graphics.Control.SetPatternPhase(new sd.SizeF(pos.X, pos.Y));
				}
				#endif

				// make current transform apply to the pattern
				var currentTransform = graphics.CurrentTransform;
				if (pattern == null || viewTransform != currentTransform)
				{
					viewTransform = currentTransform;
					SetPattern();
				}

				graphics.Control.SetFillPattern(pattern, alpha);
			}

			public float Opacity
			{
				get { return alpha[0]; }
				set { alpha[0] = value; }
			}

			public CGAffineTransform Transform
			{
				get { return transform; }
				set
				{
					transform = value;
					pattern = null;
				}
			}

			void DrawPattern(CGContext context)
			{
				var start = new sd.PointF(0, 0);
				var end = start + sectionSize;

				context.ClipToRect(sd.RectangleF.Inflate(new sd.RectangleF(start, tileSize), 4, 4));

				if (Wrap == GradientWrapMode.Reflect)
				{
					for (int i = 0; i < 2; i++)
					{
						context.DrawLinearGradient(Gradient, start, end, 0);
						context.DrawLinearGradient(InverseGradient, end, end + sectionSize, 0);
						start = start + sectionSize + sectionSize;
						end = end + sectionSize + sectionSize;
					}
				}
				else
				{
					for (int i = 0; i < 2; i++)
					{
						context.DrawLinearGradient(Gradient, start, end, 0);
						start = start + sectionSize;
						end = end + sectionSize;
					}
				}
			}

			void SetPattern()
			{
				sectionSize = new sd.SizeF((EndPoint.X - StartPoint.X) + 1, (EndPoint.Y - StartPoint.Y) + 1);
				if (Wrap == GradientWrapMode.Reflect)
					tileSize = new sd.SizeF(sectionSize.Width * 4, sectionSize.Height * 4);
				else
					tileSize = new sd.SizeF(sectionSize.Width * 2, sectionSize.Height * 2);
				var rect = new sd.RectangleF(StartPoint, tileSize);
				var t = CGAffineTransform.Multiply(transform, viewTransform);
				pattern = new CGPattern(rect, t, rect.Width, rect.Height, CGPatternTiling.NoDistortion, true, DrawPattern);
			}
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new BrushObject
			{
				Gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor []
				{
					startColor.ToCGColor(),
					endColor.ToCGColor()
				}),
				InverseGradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor []
				{
					endColor.ToCGColor(),
					startColor.ToCGColor()
				}),
				StartPoint = startPoint.ToSD(),
				EndPoint = endPoint.ToSD()
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			return null;
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform.ToCG();
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Wrap;
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).Wrap = gradientWrap;
		}

		public override void Apply(object control, GraphicsHandler graphics)
		{
			((BrushObject)control).Apply(graphics);
		}
	}
}

