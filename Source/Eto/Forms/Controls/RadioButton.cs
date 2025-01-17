using System;

namespace Eto.Forms
{
	/// <summary>
	/// Control to present a button that can be toggled on
	/// </summary>
	/// <remarks>
	/// The RadioButton works with other radio buttons to present a list of options that the user can select from.
	/// When a radio button is toggled on, all others that are linked together will be toggled off.
	/// 
	/// To link radio buttons together, use the <see cref="C:Eto.Forms.RadioButton(RadioButton)"/> constructor
	/// to specify the controller radio button, which can be created with the default constructor.
	/// </remarks>
	/// <seealso cref="RadioButtonList"/>
	[Handler(typeof(RadioButton.IHandler))]
	public class RadioButton : TextControl
	{
		/// <summary>
		/// Occurs when the <see cref="Checked"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged;

		/// <summary>
		/// Occurs when the user clicks the radio button.
		/// </summary>
		public event EventHandler<EventArgs> Click;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		public RadioButton()
		{
			Handler.Create(null);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		/// <param name="controller">Controller radio button to link to, or null if no controller.</param>
		public RadioButton(RadioButton controller = null)
		{
			Handler.Create(controller);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use RadioButton(RadioButton) instead")]
		public RadioButton(RadioButton controller = null, Generator generator = null)
			: this(generator, typeof(IHandler), controller)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButton"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="controller">Controller.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected RadioButton(Generator generator, Type type, RadioButton controller, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create(controller);
			Initialize();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioButton"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public virtual bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Callback interface for the <see cref="RadioButton"/>
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			void OnClick(RadioButton widget, EventArgs e);

			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			void OnCheckedChanged(RadioButton widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="RadioButton"/>
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the click event.
			/// </summary>
			public void OnClick(RadioButton widget, EventArgs e)
			{
				widget.OnClick(e);
			}

			/// <summary>
			/// Raises the checked changed event.
			/// </summary>
			public void OnCheckedChanged(RadioButton widget, EventArgs e)
			{
				widget.OnCheckedChanged(e);
			}
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Handler interface for the <see cref="RadioButton"/>
		/// </summary>
		/// <remarks>
		/// When using this handler, you must call <see cref="Eto.Widget.Initialize"/> in the constructor.
		/// </remarks>
		[AutoInitialize(false)]
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Used when creating a new instance of the RadioButton to specify the controller
			/// </summary>
			/// <param name="controller">Controller radio button to link to, or null if no controller.</param>
			void Create(RadioButton controller);

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioButton"/> is checked.
			/// </summary>
			/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
			bool Checked { get; set; }
		}
	}
}
