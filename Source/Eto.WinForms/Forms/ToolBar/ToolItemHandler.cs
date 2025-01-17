using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms
{
	public interface IToolBarItemHandler
	{
		void CreateControl(ToolBarHandler handler, int index);
	}

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler, IToolBarItemHandler
		where TControl : SWF.ToolStripItem
		where TWidget : ToolItem
	{
		Image image;
		int imageSize = 16;

		public abstract void CreateControl(ToolBarHandler handler, int index);

		public virtual void InvokeButton()
		{
		}

		public int ImageSize
		{
			get { return imageSize; }
			set
			{
				imageSize = value;
				Control.Image = image.ToSD(imageSize);
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public string ToolTip
		{
			get { return Control.ToolTipText; }
			set { Control.ToolTipText = value; }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToSD(imageSize);
			}
		}

		public abstract bool Enabled { get; set; }

		public void CreateFromCommand(Command command)
		{
		}
	}
}
