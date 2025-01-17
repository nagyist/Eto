using Eto.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

#if OSX
namespace Eto.Mac.Drawing
#elif IOS

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using NSFont = MonoTouch.UIKit.UIFont;

namespace Eto.iOS.Drawing
#endif
{
	public static class FontExtensions
	{
		#if OSX
		static readonly NSString ForegroundColorAttribute = NSAttributedString.ForegroundColorAttributeName;
		#elif IOS
		static readonly NSString ForegroundColorAttribute = UIStringAttributeKey.ForegroundColor;
		static readonly Selector selSetSize = new Selector("setSize:");
		#endif

		public static NSFont ToNSFont(this Font font)
		{
			return font == null ? null : ((FontHandler)font.Handler).Control;
		}

		public static NSDictionary Attributes(this Font font)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				return handler.Attributes;
			}
			return null;
		}

		public static void Apply(this Font font, NSMutableAttributedString str)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				str.AddAttributes(handler.Attributes, new NSRange(0, str.Length));
			}
		}

		public static void Apply(this Font font, NSMutableDictionary dic)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				foreach (var item in handler.Attributes)
					dic.Add(item.Key, item.Value);
			}
		}

		public static NSAttributedString AttributedString(this Font font, NSAttributedString attributedString)
		{
			if (font != null)
			{
				var str = attributedString as NSMutableAttributedString ?? new NSMutableAttributedString(attributedString);
				font.Apply(str);
				return str;
			}
			return attributedString;
		}

		public static NSMutableAttributedString ToMutable(this NSAttributedString attributedString, string text)
		{
			if (attributedString != null && attributedString.Length > 0)
			{
				NSRange range;
				return new NSMutableAttributedString(text, attributedString.GetAttributes(0, out range));
			}

			return new NSMutableAttributedString(text);
		}

		public static NSMutableAttributedString AttributedString(this Font font, string text, NSAttributedString attributedString = null)
		{
			var mutable = attributedString.ToMutable(text);
			font.Apply(mutable);

			return mutable;
		}

		public static float LineHeight(this NSFont font)
		{
			#if OSX
			return layout.DefaultLineHeightForFont(font);
			#elif IOS
			return font.LineHeight;
			#endif
			/**
			var leading = Math.Floor (Math.Max (0, font.Leading) + 0.5f);
			var lineHeight = (float)(Math.Floor(font.Ascender + 0.5f) - Math.Floor (font.Descender + 0.5f) + leading);

			if (leading > 0)
				return lineHeight;
			else
				return (float)(lineHeight + Math.Floor(0.2 * lineHeight + 0.5));
			/**/
		}

		static readonly NSTextStorage storage;
		static readonly NSLayoutManager layout;
		static readonly NSTextContainer container;

		public static NSLayoutManager SharedLayout { get { return layout; } }

		public static SizeF MeasureString(string text, Font font = null, SizeF? availableSize = null)
		{
			return MeasureString(font.AttributedString(text), availableSize);
		}

		public static SizeF MeasureString(NSAttributedString str, SizeF? availableSize = null)
		{
			SetContainerSize(availableSize);
			storage.SetString(str);
			return layout.BoundingRectForGlyphRange(new NSRange(0, str.Length), container).Size.ToEto();
		}

		public static void DrawString(NSAttributedString str, PointF point, SizeF? availableSize = null)
		{
			SetContainerSize(availableSize);
			storage.SetString(str);
			layout.DrawGlyphs(new NSRange(0, str.Length), point.ToSD());
		}

		public static void DrawString(string text, PointF point, Color color, Font font = null, SizeF? availableSize = null)
		{
			var str = font.AttributedString(text);
			str.AddAttribute(ForegroundColorAttribute, color.ToNSUI(), new NSRange(0, text.Length));
			DrawString(str, point, availableSize);
		}

		static void SetContainerSize(SizeF? availableSize)
		{
			#if OSX
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToSD();
			#elif IOS
			if (container.RespondsToSelector(selSetSize))
				container.Size = (availableSize ?? SizeF.MaxValue).ToSD();
			#endif
		}

		static FontExtensions()
		{
			storage = new NSTextStorage();
			layout = new NSLayoutManager { UsesFontLeading = true };
			#if OSX
			layout.BackgroundLayoutEnabled = false;
			#endif
			container = new NSTextContainer { LineFragmentPadding = 0f };
			layout.UsesFontLeading = true;
			layout.AddTextContainer(container);
			storage.AddLayoutManager(layout);
		}
	}
}