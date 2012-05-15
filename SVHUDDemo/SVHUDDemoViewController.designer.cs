// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SVHUDDemo
{
	[Register ("SVHUDDemoViewController")]
	partial class SVHUDDemoViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton showButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton showStatusButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton dismissButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton dismissSuccess { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton dismissError { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (showButton != null) {
				showButton.Dispose ();
				showButton = null;
			}

			if (showStatusButton != null) {
				showStatusButton.Dispose ();
				showStatusButton = null;
			}

			if (dismissButton != null) {
				dismissButton.Dispose ();
				dismissButton = null;
			}

			if (dismissSuccess != null) {
				dismissSuccess.Dispose ();
				dismissSuccess = null;
			}

			if (dismissError != null) {
				dismissError.Dispose ();
				dismissError = null;
			}
		}
	}
}
