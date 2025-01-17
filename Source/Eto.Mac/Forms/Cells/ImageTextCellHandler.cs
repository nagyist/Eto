using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<NSTextFieldCell, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler
	{
		public class EtoCell : MacImageListItemCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell ()
			{
			}
			
			public EtoCell (IntPtr handle) : base(handle)
			{
			}
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, MacCommon.CopyWithZoneHandle, zone);
				return new EtoCell (ptr) { Handler = Handler };
			}
		}
		
		public ImageTextCellHandler ()
		{
			Control = new EtoCell { 
				Handler = this, 
				UsesSingleLineMode = true
			};
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color.ToNSUI ();
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.BackgroundColor.ToEto ();
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.TextColor = color.ToNSUI ();
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.TextColor.ToEto ();
		}

		public override NSObject GetObjectValue (object dataItem)
		{
			var result = new MacImageData();
			if (Widget.TextBinding != null) {
				result.Text = (NSString)Convert.ToString (Widget.TextBinding.GetValue (dataItem));
			}
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				result.Image = image != null ? ((IImageSource)image.Handler).GetImage () : null;
			}
			else result.Image = null;
			return result;
		}
		
		public override void SetObjectValue (object dataItem, NSObject value)
		{
			if (Widget.TextBinding != null) {
				var str = value as NSString;
				if (str != null)
					Widget.TextBinding.SetValue (dataItem, str.ToString());
				else
					Widget.TextBinding.SetValue (dataItem, null);
			}
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize, NSCell cell)
		{
			var val = value as MacImageData;
			if (val == null) return 0;
			
			var font = cell.Font ?? NSFont.BoldSystemFontOfSize (NSFont.SystemFontSize);
			var str = val.Text;
			var attrs = NSDictionary.FromObjectAndKey (font, NSAttributedString.FontAttributeName);
			
			var size = str.StringSize (attrs).Width + 4 + 16 + MacImageListItemCell.ImagePadding * 2; // for border + image
			return size;
			
		}
	}
}

