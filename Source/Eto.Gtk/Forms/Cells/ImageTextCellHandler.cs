using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<Gtk.CellRendererText, ImageTextCell>, ImageTextCell.IHandler
	{
		readonly Gtk.CellRendererPixbuf imageCell;
		int imageDataIndex;
		int textDataIndex;

		class Renderer : Gtk.CellRendererText
		{
			WeakReference handler;
			public ImageTextCellHandler Handler { get { return (ImageTextCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }
			#if GTK2
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkTextCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Item, Row));
			
				// calling base crashes on windows /w gtk 2.12.9
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererText.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
			#else
			protected override void OnGetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.OnGetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}
			
			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<Renderer> (this, Handler.Column.Widget, Item, Row));
				base.OnRender (cr, widget, background_area, cell_area, flags);
			}
#endif
		}

		class ImageRenderer : Gtk.CellRendererPixbuf
		{
			public ImageTextCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }
			#if GTK2
			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<ImageRenderer>(this, Handler.Column.Widget, Item, Row));

				// calling base crashes on windows /w gtk 2.12.9
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererPixbuf.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
			#else
			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<ImageRenderer> (this, Handler.Column.Widget, Item, Row));
				base.OnRender (cr, widget, background_area, cell_area, flags);
			}
#endif
		}

		public ImageTextCellHandler()
		{
			imageCell = new ImageRenderer { Handler = this };
			Control = new Renderer { Handler = this };
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Edited += Connector.HandleEdited;
		}

		protected new ImageTextCellEventConnector Connector { get { return (ImageTextCellEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ImageTextCellEventConnector();
		}

		protected class ImageTextCellEventConnector : WeakConnector
		{
			public new ImageTextCellHandler Handler { get { return (ImageTextCellHandler)base.Handler; } }

			public void HandleEdited(object o, Gtk.EditedArgs args)
			{
				Handler.SetValue(args.Path, args.NewText);
			}

			public void HandleEndCellEditing(object o, Gtk.EditedArgs args)
			{
				Handler.Source.EndCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}

		public override void AddCells(Gtk.TreeViewColumn column)
		{
			column.PackStart(imageCell, false);
			column.PackStart(Control, true);
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			imageDataIndex = SetColumnMap(dataIndex);
			Column.Control.AddAttribute(imageCell, "pixbuf", dataIndex++);
			textDataIndex = SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "text", dataIndex++);
			BindBase(ref dataIndex);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
			Control.Editable = editable;
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.TextBinding != null)
			{
				Widget.TextBinding.SetValue(dataItem, Convert.ToString(value));
			}
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (dataColumn == imageDataIndex)
			{
				if (Widget.ImageBinding != null)
				{
					var ret = Widget.ImageBinding.GetValue(dataItem);
					var image = ret as Image;
					if (image != null)
						return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf(new Size(16, 16)));
				}
				return new GLib.Value((Gdk.Pixbuf)null);
			}
			if (dataColumn == textDataIndex)
			{
				var ret = Widget.TextBinding.GetValue(dataItem);
				if (ret != null)
					return new GLib.Value(Convert.ToString(ret));
			}
			return new GLib.Value((string)null);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditedEvent:
					Control.Edited += Connector.HandleEndCellEditing;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

