using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class ImageViewCellHandler : CellHandler<swc.DataGridColumn, ImageViewCell>, ImageViewCell.IHandler
	{
		public static int ImageSize = 16;

		object GetValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var image = Widget.Binding.GetValue (dataItem) as Image;
				if (image != null)
					return ((IWpfImage)image.Handler).GetImageClosestToSize (ImageSize);
			}
			return null;
		}

		public class Column : swc.DataGridColumn
		{
			public ImageViewCellHandler Handler { get; set; }

			swc.Image Image (swc.DataGridCell cell)
			{
				var image = new swc.Image { MaxWidth = 16, MaxHeight = 16, StretchDirection = swc.StretchDirection.DownOnly, Margin = new sw.Thickness (0, 2, 2, 2) };
				image.DataContextChanged += (sender, e) => {
					var img = sender as swc.Image;
					img.Source = Handler.GetValue (img.DataContext) as swm.ImageSource;
					Handler.FormatCell (img, cell, img.DataContext);
				};
				return image;
			}

			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				return Handler.SetupCell (Image(cell));
			}

			protected override object PrepareCellForEdit (sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("control");
				return base.PrepareCellForEdit (control, editingEventArgs);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				return Handler.SetupCell (Image (cell));
			}
		}

		public ImageViewCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}
