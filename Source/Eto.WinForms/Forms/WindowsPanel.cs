using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms
{
	public abstract class WindowsPanel<TControl, TWidget, TCallback> : WindowsContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl : swf.Control
		where TWidget : Panel
		where TCallback : Panel.ICallback
	{
		Control content;

		protected override void Initialize()
		{
			base.Initialize();
			Padding = Panel.DefaultPadding;
			ContainerContentControl.SuspendLayout();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			ContainerContentControl.ResumeLayout();
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			ContainerContentControl.SuspendLayout();
		}

		public virtual swf.Control ContainerContentControl
		{
			get { return Control; }
		}

		protected virtual Size ContentPadding { get { return Size.Empty; } }

		public override Size ParentMinimumSize
		{
			get
			{
				return base.ParentMinimumSize;
			}
			set
			{
				var control = content.GetWindowsHandler();
				if (control != null)
				{
					control.ParentMinimumSize = value - ContentPadding;
				}
				base.ParentMinimumSize = value;
			}
		}

		public override Size GetPreferredSize(Size availableSize)
		{
			var desiredSize = base.GetPreferredSize(availableSize);

			var handler = content.GetWindowsHandler();
			if (handler != null)
			{
				var desiredContentSize = handler.GetPreferredSize(availableSize);
				if (!handler.XScale)
				{
					if (desiredSize.Width > 0)
						desiredSize.Width = Math.Max(desiredSize.Width, desiredContentSize.Width);
					else
						desiredSize.Width = desiredContentSize.Width;
				}

				if (!handler.YScale)
				{
					if (desiredSize.Height > 0)
						desiredSize.Height = Math.Max(desiredSize.Height, desiredContentSize.Height);
					else
						desiredSize.Height = desiredContentSize.Height;
				}
			}
			return desiredSize + Padding.Size;
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetContentScale(xscale, yscale);
		}

		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			if (content != null)
				content.SetScale(xscale, yscale);
		}

		public Padding Padding
		{
			get { return ContainerContentControl.Padding.ToEto(); }
			set { ContainerContentControl.Padding = value.ToSWF(); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (Widget.Loaded)
					SuspendLayout();

				if (content != null)
				{
					var contentHandler = content.GetWindowsHandler();
					contentHandler.SetScale(false, false);
					contentHandler.ContainerControl.Parent = null;
				}

				content = value;
				if (content != null)
				{
					SetContentScale(XScale, YScale);
					SetContent(content.GetContainerControl());
				}

				if (Widget.Loaded)
					ResumeLayout();
			}
		}

		protected virtual void SetContent(swf.Control contentControl)
		{
			contentControl.Dock = swf.DockStyle.Fill;
			ContainerContentControl.Controls.Add(contentControl);
		}
	}
}
