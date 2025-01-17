using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Interface elment to group controls inside a box with an optional title
	/// </summary>
	[Handler(typeof(GroupBox.IHandler))]
	public class GroupBox : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GroupBox"/> class.
		/// </summary>
		public GroupBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GroupBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public GroupBox(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GroupBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected GroupBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the font used for the title
		/// </summary>
		/// <value>The title font.</value>
		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}

		/// <summary>
		/// Gets or sets the title text.
		/// </summary>
		/// <value>The title text.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="GroupBox"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets the font used for the title
			/// </summary>
			/// <value>The title font.</value>
			Font Font { get; set; }

			/// <summary>
			/// Gets or sets the title text.
			/// </summary>
			/// <value>The title text.</value>
			string Text { get; set; }
		}
	}
}
