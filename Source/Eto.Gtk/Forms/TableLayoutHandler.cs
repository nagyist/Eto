using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.GtkSharp
{
	public class TableLayoutHandler : GtkContainer<Gtk.Table, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		const Gtk.AttachOptions SCALED_OPTIONS = Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
		Gtk.Alignment align;
		Gtk.EventBox box;
		bool[] columnScale;
		int lastColumnScale;
		bool[] rowScale;
		int lastRowScale;
		Control[,] controls;
		Gtk.Widget[,] blank;

		public override Gtk.Widget ContainerControl
		{
			get { return box; }
		}

		public Size Spacing
		{
			get { return new Size((int)Control.ColumnSpacing, (int)Control.RowSpacing); }
			set
			{ 
				Control.ColumnSpacing = (uint)value.Width;
				Control.RowSpacing = (uint)value.Height;
			}
		}

		public Padding Padding
		{
			get
			{
				uint top, bottom, left, right;
				align.GetPadding(out top, out bottom, out left, out right);
				return new Padding((int)left, (int)top, (int)right, (int)bottom);
			}
			set
			{
				align.SetPadding((uint)value.Top, (uint)value.Bottom, (uint)value.Left, (uint)value.Right);
			}
		}

		public void Add(Control child, int x, int y)
		{
			Attach(child, x, y);
			if (child != null)
			{
				var widget = child.GetContainerWidget();
				widget.ShowAll();
			}
		}

		public void Move(Control child, int x, int y)
		{
			Attach(child, x, y);
		}

		Gtk.AttachOptions GetColumnOptions(int column)
		{
			var scale = columnScale[column] || column == lastColumnScale;
			return scale ? SCALED_OPTIONS : Gtk.AttachOptions.Fill;
		}

		Gtk.AttachOptions GetRowOptions(int row)
		{
			var scale = rowScale[row] || row == lastRowScale;
			return scale ? SCALED_OPTIONS : Gtk.AttachOptions.Fill;
		}

		public void CreateControl(int cols, int rows)
		{
			columnScale = new bool[cols];
			lastColumnScale = cols - 1;
			rowScale = new bool[rows];
			lastRowScale = rows - 1;
			align = new Gtk.Alignment(0, 0, 1.0F, 1.0F);
			Control = new Gtk.Table((uint)rows, (uint)cols, false);
			controls = new Control[rows, cols];
			blank = new Gtk.Widget[rows, cols];
			align.Add(Control);
			box = new Gtk.EventBox { Child = align };

			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = lastColumnScale;
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			AttachColumn(column);
			if (lastScale != lastColumnScale && column != columnScale.Length - 1)
			{
				AttachColumn(columnScale.Length - 1);
			}
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		void AttachColumn(int column)
		{
			for (int y = 0; y < controls.GetLength (0); y++)
			{
				Attach(controls[y, column], column, y);
			}
		}

		void AttachRow(int row)
		{
			for (int x = 0; x < controls.GetLength (1); x++)
			{
				Attach(controls[row, x], x, row);
			}
		}

		bool Attach(Control child, int x, int y)
		{
			var old = controls[y, x];
			if (old != null && !object.ReferenceEquals(old, child))
			{
				var widget = old.GetContainerWidget();
				if (widget.Parent != null)
					Control.Remove(widget);
			}

			if (child != null)
			{
				var blankWidget = blank[y, x];
				if (blankWidget != null)
				{
					if (blankWidget.Parent != null)
						Control.Remove(blankWidget);
					blank[y, x] = null;
				}

				controls[y, x] = child;
				var widget = child.GetContainerWidget();
				if (widget.Parent != null)
					((Gtk.Container)widget.Parent).Remove(widget);
				Control.Attach(widget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions(x), GetRowOptions(y), 0, 0);
				widget.ShowAll();
				return true;
			}
			else
			{
				controls[y, x] = null;
				var blankWidget = blank[y, x];
				if (blankWidget == null)
					blankWidget = blank[y, x] = new Gtk.VBox();
				else if (blankWidget.Parent != null)
					Control.Remove(blankWidget);
				Control.Attach(blankWidget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions(x), GetRowOptions(y), 0, 0);
			}
			return false;
		}

		public void Remove(Control child)
		{
			for (int y = 0; y<controls.GetLength(0); y++)
			{
				for (int x = 0; x<controls.GetLength(1); x++)
				{
					if (object.ReferenceEquals(controls[y, x], child))
					{
						controls[y, x] = null;
						var widget = child.GetContainerWidget();
						Control.Remove(widget);

						var blankWidget = blank[y, x] = new Gtk.VBox();
						Control.Attach(blankWidget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions(x), GetRowOptions(y), 0, 0);
						return;
					}
				}
			}
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = lastRowScale;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
			AttachRow(row);
			if (lastScale != lastRowScale && row != rowScale.Length - 1)
			{
				AttachRow(rowScale.Length - 1);
			}
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}

		public void Update()
		{
			Control.ResizeChildren();
		}
	}
}
