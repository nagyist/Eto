using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public interface IDataViewHandler
	{
		bool ShowHeader { get; }

		NSTableView Table { get; }

		object GetItem(int row);

		int RowCount { get; }

		System.Drawing.RectangleF GetVisibleRect();

		void OnCellFormatting(GridColumn column, object item, int row, NSCell cell);
	}

	public interface IDataColumnHandler
	{
		void Setup(int column);

		NSObject GetObjectValue(object dataItem);

		void SetObjectValue(object dataItem, NSObject val);

		GridColumn Widget { get; }

		IDataViewHandler DataViewHandler { get; }
	}

	public class GridColumnHandler : MacObject<NSTableColumn, GridColumn, GridColumn.ICallback>, GridColumn.IHandler, IDataColumnHandler
	{
		Cell dataCell;
		Font font;

		public IDataViewHandler DataViewHandler { get; private set; }

		public int Column { get; private set; }

		public GridColumnHandler()
		{
			Control = new NSTableColumn();
			Control.ResizingMask = NSTableColumnResizing.None;
			Sortable = false;
			HeaderText = string.Empty;
			Editable = false;
			AutoSize = true;
			DataCell = new TextBoxCell();
		}

		public void Loaded(IDataViewHandler handler, int column)
		{
			Column = column;
			DataViewHandler = handler;
		}

		public void Resize(bool force = false)
		{
			var handler = DataViewHandler;
			if (AutoSize && handler != null)
			{
				float width = Control.DataCell.CellSize.Width;
				var outlineView = handler.Table as NSOutlineView;
				if (handler.ShowHeader)
					width = Math.Max(Control.HeaderCell.CellSize.Width, width);
					
				if (dataCell != null)
				{
					/* Auto size based on visible cells only */
					var rect = handler.GetVisibleRect();
					var range = handler.Table.RowsInRect(rect);

					var cellSize = Control.DataCell.CellSize;
					var dataCellHandler = ((ICellHandler)dataCell.Handler);
					for (int i = range.Location; i < range.Location + range.Length; i++)
					{
						var cellWidth = GetRowWidth(dataCellHandler, i, cellSize) + 4;
						if (outlineView != null && Column == 0)
						{
							cellWidth += (outlineView.LevelForRow(i) + 1) * outlineView.IndentationPerLevel;
						}
						width = Math.Max(width, cellWidth);
					}
				}
				if (force || width > Control.Width)
					Control.Width = width;
			}
		}

		protected virtual float GetRowWidth(ICellHandler cell, int row, System.Drawing.SizeF cellSize)
		{
			var item = DataViewHandler.GetItem(row);
			var val = GetObjectValue(item);
			return cell.GetPreferredSize(val, cellSize, row, item);
		}

		public string HeaderText
		{
			get { return Control.HeaderCell.StringValue; }
			set { Control.HeaderCell.StringValue = value; }
		}

		public bool Resizable
		{
			get
			{
				return Control.ResizingMask.HasFlag(NSTableColumnResizing.UserResizingMask);
			}
			set
			{
				if (value)
					Control.ResizingMask |= NSTableColumnResizing.UserResizingMask;
				else
					Control.ResizingMask &= ~NSTableColumnResizing.UserResizingMask;
			}
		}

		public bool AutoSize
		{
			get;
			set;
		}

		public bool Sortable { get; set; }

		public bool Editable
		{
			get
			{
				return Control.Editable;
			}
			set
			{
				Control.Editable = value;
				if (dataCell != null)
				{
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.Editable = value;
				}
			}
		}

		public int Width
		{
			get { return (int)Control.Width; }
			set { Control.Width = value; }
		}

		public bool Visible
		{
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				dataCell = value;
				if (dataCell != null)
				{
					var editable = Editable;
					var cellHandler = (ICellHandler)dataCell.Handler;
					Control.DataCell = cellHandler.Control;
					cellHandler.ColumnHandler = this;
					cellHandler.Editable = editable;
				}
				else
					Control.DataCell = null;
			}
		}

		public void Setup(int column)
		{
			Column = column;
			Control.Identifier = new NSString(column.ToString());
		}

		public NSObject GetObjectValue(object dataItem)
		{
			return ((ICellHandler)dataCell.Handler).GetObjectValue(dataItem);
		}

		public void SetObjectValue(object dataItem, NSObject val)
		{
			((ICellHandler)dataCell.Handler).SetObjectValue(dataItem, val);
		}

		GridColumn IDataColumnHandler.Widget
		{
			get { return Widget; }
		}

		public Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.DataCell.Font));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
				{
					var fontHandler = (FontHandler)font.Handler;
					Control.DataCell.Font = fontHandler.Control;
				}
				else
					Control.DataCell.Font = null;
			}
		}
	}
}

