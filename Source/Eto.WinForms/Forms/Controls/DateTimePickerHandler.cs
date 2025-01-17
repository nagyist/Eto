using System;
using Eto.Forms;
using System.Globalization;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class DateTimePickerHandler : WindowsControl<swf.DateTimePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		public DateTimePickerHandler()
		{
			Control = new swf.DateTimePicker();
			Control.ShowCheckBox = true;
			Mode = DateTimePicker.DefaultMode;
			Value = null;
			Control.ValueChanged += delegate
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
		}

		public DateTimePickerMode Mode
		{
			get
			{
				switch (Control.Format)
				{
					case swf.DateTimePickerFormat.Long:
						return DateTimePickerMode.DateTime;
					case swf.DateTimePickerFormat.Short:
						return DateTimePickerMode.Date;
					case swf.DateTimePickerFormat.Time:
						return DateTimePickerMode.Time;
					default:
						throw new NotImplementedException();
				}
			}
			set
			{
				switch (value)
				{
					case DateTimePickerMode.DateTime:
						Control.Format = swf.DateTimePickerFormat.Custom;
						var format = CultureInfo.CurrentUICulture.DateTimeFormat;
						Control.CustomFormat = format.ShortDatePattern + " " + format.LongTimePattern;
						break;
					case DateTimePickerMode.Date:
						Control.Format = swf.DateTimePickerFormat.Short;
						break;
					case DateTimePickerMode.Time:
						Control.Format = swf.DateTimePickerFormat.Time;
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public DateTime MinDate
		{
			get
			{
				return Control.MinDate == swf.DateTimePicker.MinimumDateTime ? DateTime.MinValue : Control.MinDate;
			}
			set
			{
				Control.MinDate = value == DateTime.MinValue ? swf.DateTimePicker.MinimumDateTime : value;
			}
		}

		public DateTime MaxDate
		{
			get
			{
				return Control.MaxDate == swf.DateTimePicker.MaximumDateTime ? DateTime.MaxValue : Control.MaxDate;
			}
			set
			{
				Control.MaxDate = value == DateTime.MaxValue ? swf.DateTimePicker.MaximumDateTime : value;
			}
		}

		public DateTime? Value
		{
			get
			{
				return !Control.Checked ? null : (DateTime?)Control.Value;
			}
			set
			{
				if (value != null)
				{
					var date = value.Value;
					if (date < MinDate) date = MinDate;
					if (date > MaxDate) date = MaxDate;
					Control.Value = date;
					Control.Checked = true;
				}
				else
					Control.Checked = false;
			}
		}
	}
}

