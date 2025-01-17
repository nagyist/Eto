using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms
{
	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Menu.IHandler
		where TControl: SWF.ToolStripItem
		where TWidget: Widget
	{

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuItem.ValidateEvent:
				// handled by parents
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
