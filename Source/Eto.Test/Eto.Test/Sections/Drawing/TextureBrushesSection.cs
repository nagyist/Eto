using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class TextureBrushesSection : Panel
	{
		public TextureBrushesSection()
		{
			var image = TestIcons.Textures;
			var drawable = new Drawable { Size = new Size(image.Size.Width, image.Size.Height * 10) };
			var drawableTarget = new DrawableTarget(drawable);
			var layout = new DynamicLayout { Padding = new Padding(10) };
			layout.AddSeparateRow(null, drawableTarget.Checkbox(), null);
			layout.Add(new Scrollable { Content = drawable });
			Content = layout;

			var renderers = new List<Action<Graphics>>();

			for (var i = 0; i < 10; ++i)
			{
				var w = image.Size.Width / 3; // same as height
				var img = image;
				if (i > 0)
					img = img.Clone(new Rectangle((i - 1) % 3 * w, (i - 1) / 3 * w, w, w));

				var brush = new TextureBrush(img);

				renderers.Add(graphics =>
				{
					var temp = brush.Transform; // save state
					brush.Transform = Matrix.FromRotation(90);
					graphics.FillRectangle(brush, new RectangleF(image.Size));
					graphics.TranslateTransform(0, image.Size.Height);
					brush.Transform = temp;
				});
			}

			drawable.Paint += (s, e) =>
			{
				var graphics = drawableTarget.BeginDraw(e);
				foreach (var renderer in renderers)
					renderer(graphics);
				drawableTarget.EndDraw(e);
			};
		}
	}
}
