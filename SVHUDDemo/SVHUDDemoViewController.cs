using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SVProgressHUDLib;

namespace SVHUDDemo {
    public partial class SVHUDDemoViewController : UIViewController {
        static bool UserInterfaceIdiomIsPhone {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public SVHUDDemoViewController ()
            : base (UserInterfaceIdiomIsPhone ? "SVHUDDemoViewController_iPhone" : "SVHUDDemoViewController_iPad", null) {
        }
        
        public override void DidReceiveMemoryWarning () {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
            
            // Release any cached data, images, etc that aren't in use.
        }
        
        public override void ViewDidLoad () {
            base.ViewDidLoad ();
            
            // Perform any additional setup after loading the view, typically from a nib.
            this.showStatusButton.TouchUpInside += (sender, e) => {
                SVProgressHUD.Show("Hello");
            };
            this.dismissButton.TouchUpInside += (sender, e) => {
                SVProgressHUD.Dismiss();
            };
            this.dismissSuccess.TouchUpInside += (sender, e) => {
                //SVProgressHUD.DismissSuccess();
            };
        }
        
        public override void ViewDidUnload () {
            base.ViewDidUnload ();
            
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
            
            ReleaseDesignerOutlets ();
        }
        
        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation) {
            // Return true for supported orientations
            if (UserInterfaceIdiomIsPhone) {
                return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
            } else {
                return toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            }
        }
    }
}

