using System;
using MonoTouch;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.iOS.Forms.Controls
{
	public interface IGridHandler
	{
		Grid Widget { get; }
		Grid.ICallback Callback { get; }
	}

	public abstract class GridHandler<TControl, TWidget, TCallback> : IosView<TControl, TWidget, TCallback>, Grid.IHandler, IGridHandler
		where TControl: UITableView
		where TWidget: Grid
		where TCallback: Grid.ICallback
	{
		Grid IGridHandler.Widget { get { return Widget; } }
		Grid.ICallback IGridHandler.Callback { get { return Callback; } }

		public new UITableViewController Controller
		{
			get { return (UITableViewController)base.Controller; }
			set { base.Controller = value; }
		}

		protected virtual UITableViewDelegate CreateDelegate()
		{
			return new GridHandlerTableDelegate(this);
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Controller = new RotatableTableViewController { Control = this.Widget };
			Control = (TControl)Controller.TableView;

			Control.Delegate = CreateDelegate ();
		}

		public void SelectRow (int row)
		{
		}

		public void UnselectRow (int row)
		{
		}

		public void SelectAll ()
		{
		}

		public void UnselectAll ()
		{
		}

		public bool ShowHeader {
			get;
			set;
		}

		public int RowHeight {
			get;
			set;
		}

		public bool AllowColumnReordering {
			get;
			set;
		}

		public bool AllowMultipleSelection {
			get;
			set;
		}

		public IEnumerable<int> SelectedRows {
			get 
			{
				var i = Control.IndexPathsForSelectedRows;
				if (i != null)
					foreach (var s in i)
						yield return s.Row;
			}
		}
	}

	public class GridHandlerTableDelegate : UITableViewDelegate
	{
		WeakReference handler;
		public IGridHandler Handler { get { return (IGridHandler)handler.Target; } set { handler = new WeakReference(value); } }

		public GridHandlerTableDelegate(IGridHandler handler)
		{
			this.Handler = handler;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
		}
	}

	internal class RotatableTableViewController : UITableViewController
	{
		WeakReference handler;
		public object Control { get { return (object)handler.Target; } set { handler = new WeakReference(value); } }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		protected override void Dispose(bool disposing) // TODO: Is this needed? RotatableViewController implements Dispose but RotatableNavigationController does not.
		{
			var c = Control as IDisposable;
			if (c != null)
			{
				c.Dispose();
				c = null;
			}
			base.Dispose(disposing);
		}
	}
}

