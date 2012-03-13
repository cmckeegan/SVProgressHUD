// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Demo
{
 [Register ("DemoViewController")]
 partial class DemoViewController
 {
     [Outlet]
     MonoTouch.UIKit.UIButton showHUDButton { get; set; }

     [Action ("showHUDButtonTouchUpInside:")]
     partial void showHUDButtonTouchUpInside (MonoTouch.UIKit.UIButton sender);

     [Action ("dismissButtonTouchUpInside:")]
     partial void dismissButtonTouchUpInside (MonoTouch.UIKit.UIButton sender);
     
     void ReleaseDesignerOutlets ()
     {
         if (showHUDButton != null) {
             showHUDButton.Dispose ();
             showHUDButton = null;
         }
     }
 }
}
