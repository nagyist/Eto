using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp
{
	public class TextAreaHandler : GtkControl<Gtk.TextView, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		bool sendSelectionChanged = true;
		readonly Gtk.ScrolledWindow scroll;
		Gtk.TextTag tag;

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}

		public override Size DefaultSize { get { return TextArea.DefaultSize; } }

		public TextAreaHandler()
		{
			scroll = new Gtk.ScrolledWindow();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new Gtk.TextView();
			Size = TextArea.DefaultSize;
			scroll.Add(Control);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Buffer.Changed += Connector.HandleBufferChanged;
					break;
				case TextArea.SelectionChangedEvent:
					Control.Buffer.MarkSet += Connector.HandleSelectionChanged;
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.Buffer.MarkSet += Connector.HandleCaretIndexChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new TextAreaConnector Connector { get { return (TextAreaConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextAreaConnector();
		}

		protected class TextAreaConnector : GtkControlConnector
		{
			Range? lastSelection;
			int? lastCaretIndex;
			public new TextAreaHandler Handler { get { return (TextAreaHandler)base.Handler; } }

			public void HandleBufferChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleSelectionChanged(object o, Gtk.MarkSetArgs args)
			{
				var handler = Handler;
				var selection = handler.Selection;
				if (handler.sendSelectionChanged && selection != lastSelection)
				{
					handler.Callback.OnSelectionChanged(handler.Widget, EventArgs.Empty);
					lastSelection = selection;
				}
			}

			public void HandleCaretIndexChanged(object o, Gtk.MarkSetArgs args)
			{
				var handler = Handler;
				var caretIndex = handler.CaretIndex;
				if (handler.sendSelectionChanged && caretIndex != lastCaretIndex)
				{
					handler.Callback.OnCaretIndexChanged(handler.Widget, EventArgs.Empty);
					lastCaretIndex = caretIndex;
				}
			}

			public void HandleApplyTag(object sender, EventArgs e)
			{
				var buffer = Handler.Control.Buffer;
				var tag = Handler.tag;
				buffer.ApplyTag(tag, buffer.StartIter, buffer.EndIter);
			}
		}

		public override string Text
		{
			get { return Control.Buffer.Text; }
			set
			{
				Control.Buffer.Text = value;
				if (tag != null)
					Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
			}
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public bool Wrap
		{
			get { return Control.WrapMode != Gtk.WrapMode.None; }
			set { Control.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None; }
		}

		public void Append(string text, bool scrollToCursor)
		{
			var end = Control.Buffer.EndIter;
			Control.Buffer.Insert(ref end, text);
			if (scrollToCursor)
			{
				var mark = Control.Buffer.CreateMark(null, end, false);
				Control.ScrollToMark(mark, 0, false, 0, 0);
			}
		}

		public string SelectedText
		{
			get
			{
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
				{
					return Control.Buffer.GetText(start, end, false);
				}
				return null;
			}
			set
			{
				sendSelectionChanged = false;
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
				{
					var startOffset = start.Offset;
					Control.Buffer.Delete(ref start, ref end);
					if (value != null)
					{
						Control.Buffer.Insert(ref start, value);
						start = Control.Buffer.GetIterAtOffset(startOffset);
						end = Control.Buffer.GetIterAtOffset(startOffset + value.Length);
						Control.Buffer.SelectRange(start, end);
					}
				}
				else if (value != null)
					Control.Buffer.InsertAtCursor(value);
				if (tag != null)
					Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				sendSelectionChanged = true;
			}
		}

		public Range Selection
		{
			get
			{
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
					return new Range(start.Offset, end.Offset - start.Offset);
				return new Range(Control.Buffer.CursorPosition, 0);
			}
			set
			{
				sendSelectionChanged = false;
				var start = Control.Buffer.GetIterAtOffset(value.Start);
				var end = Control.Buffer.GetIterAtOffset(value.Start + value.Length);
				Control.Buffer.SelectRange(start, end);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				sendSelectionChanged = true;
			}
		}

		public void SelectAll()
		{
			Control.Buffer.SelectRange(Control.Buffer.StartIter, Control.Buffer.EndIter);
		}

		public int CaretIndex
		{
			get { return Control.Buffer.GetIterAtMark(Control.Buffer.InsertMark).Offset; }
			set
			{
				var ins = Control.Buffer.GetIterAtOffset(value);
				Control.Buffer.SelectRange(ins, ins);
			}
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				if (value != null)
				{
					if (tag == null)
					{
						tag = new Gtk.TextTag("font");
						Control.Buffer.TagTable.Add(tag);
						Control.Buffer.Changed += Connector.HandleApplyTag;
						Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
					}
					value.Apply(tag);
				}
				else
				{
					Control.Buffer.RemoveAllTags(Control.Buffer.StartIter, Control.Buffer.EndIter);
				}
			}
		}
	}
}
