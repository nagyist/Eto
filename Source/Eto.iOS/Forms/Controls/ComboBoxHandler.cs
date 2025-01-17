using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.Drawing;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	public class ComboBoxHandler : BasePickerHandler<ComboBox, ComboBox.ICallback, UIPickerView>, ComboBox.IHandler
	{
		CollectionHandler collection;
		int selectedIndex = -1;

		class DataSource : UIPickerViewDataSource
		{
			WeakReference handler;

			public ComboBoxHandler Handler { get { return (ComboBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override int GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override int GetRowsInComponent(UIPickerView pickerView, int component)
			{
				var data = Handler.collection;
				return data != null ? data.Count : 0;
			}
		}

		class Delegate : UIPickerViewDelegate
		{
			WeakReference handler;

			public ComboBoxHandler Handler { get { return (ComboBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override string GetTitle(UIPickerView pickerView, int row, int component)
			{
				var data = Handler.collection;
				return data != null ? Handler.Widget.TextBinding.GetValue(data.ElementAt(row)) : string.Empty;
			}
		}

		public override UIPickerView CreatePicker()
		{
			var picker = new UIPickerView();
			picker.ShowSelectionIndicator = true;
			picker.DataSource = new DataSource { Handler = this };
			picker.Delegate = new Delegate { Handler = this };
			return picker;
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
			}

			public override void AddItem(object item)
			{
			}

			public override void InsertItem(int index, object item)
			{
			}

			public override void RemoveItem(int index)
			{
				if (Handler.SelectedIndex == index)
				{
					Handler.SelectedIndex = -1;
				}
			}

			public override void RemoveAllItems()
			{
				Handler.SelectedIndex = -1;
			}
		}

		public int SelectedIndex
		{
			get	{ return selectedIndex; }
			set
			{
				if (value != selectedIndex)
				{
					selectedIndex = value;
					UpdateText();
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		protected override string GetTextValue()
		{
			if (collection != null && selectedIndex >= 0 && selectedIndex < collection.Count)
			{
				var item = collection.ElementAt(selectedIndex);
				return Widget.TextBinding.GetValue(item);
			}
			return null;
		}


		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				var index = selectedIndex;
				selectedIndex = -1;
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				SelectedIndex = index;
			}
		}

		protected override void UpdateValue(UIPickerView picker)
		{
			SelectedIndex = picker.SelectedRowInComponent(0);
		}

		protected override void UpdatePicker(UIPickerView picker)
		{
			picker.ReloadAllComponents();
			picker.Select(Math.Max(0, SelectedIndex), 0, false);
		}
	}
}
