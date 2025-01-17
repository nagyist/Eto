Eto.Forms
=========
### A cross platform desktop and mobile user interface framework

Links
-----

* Join the [forums](http://groups.google.com/group/eto-forms)
* Chat in [#eto.forms](http://chat.mibbit.com/?server=irc.gimp.org&channel=%23eto.forms) on irc.gimp.org
* Download using [nuget](https://www.nuget.org/packages/Eto.Forms.Sample/) with Visual Studio or [Xamarin Studio nuget addin](https://github.com/mrward/monodevelop-nuget-addin)


Description
-----------

This framework can be used to build applications that run across multiple platforms using their native toolkit, with an easy to use API. This will make your applications look and work as a native application on all platforms, using a single UI codebase.

For advanced scenarios, you can take advantage of each platform's capabilities by wrapping your common UI in a larger application, or even create your own high-level controls with a custom implementations per platform.

This framework currently supports creating Desktop applications that work across Windows Forms, WPF, MonoMac, and GTK#.
There is a Mobile/iOS port in the works, but is considered incomplete.

This framework was built so that using it in .NET is natural. For example, a simple hello-world application might look like:

	public class MyForm : Form
	{
		public MyForm ()
		{
			Text = "My Cross-Platform App";
			Size = new Size (200, 200);
			Content = new Label { Text = "Hello World!" };
		}
		
		[STAThread]
		static void Main () {
			var app = new Application();
			app.Initialized += delegate {
				app.MainForm = new MyForm ();
				app.MainForm.Show ();
			};
			app.Run ();
		}
	}

Applications
------------
* [Manager](http://www.manager.io) - Accounting Software
* [PabloDraw](http://picoe.ca/products/pablodraw/alpha) - Character based drawing application
* [JabbR.Desktop](https://github.com/JabbR/JabbR.Desktop) - JabbR client
* [Notedown](https://github.com/modmonkeys/Notedown) - Note taking application
* [Eto.Test](https://github.com/picoe/Eto/tree/master/Source/Eto.Test) - Application to test the functionality of each widget

Namespaces
----------

* [Eto.Forms](https://github.com/picoe/Eto/tree/master/Source/Eto/Forms) - User interface 
* [Eto.Drawing](https://github.com/picoe/Eto/tree/master/Source/Eto/Drawing) - Drawing/graphical routines
* Eto.Platform.[Platform] - platform implementations

Currently supported targets
---------------------------

* iOS using Xamarin.iOS
* OS X: MonoMac or Xamarin.Mac
* Linux: GTK# 2 or 3
* Windows: Windows Forms (using GDI or Direct2D) or WPF
	
Under development
-----------------

These platforms are currently in development. Any eager bodies willing to help feel free to do so!

* Android using Xamarin.Android (Eto.Platform.Android)
* Windows 8.x and Windows Phone (Eto.Platform.Xaml)
