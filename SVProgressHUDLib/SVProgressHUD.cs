using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

// Conversion of SVProgressHUD by Sam Vermette
namespace SVProgressHUDLib {
    public enum SVProgressHUDMask {
        None, // allow user interactions while HUD is displayed
        Clear, // don't allow
        Black, // don't allow and dim the UI in the back of the HUD
        Gradient // don't allow and dim the UI with a a-la-alert-view bg gradient
    }
 
    public class SVProgressHUD: UIView {
        private static SVProgressHUD sharedView;
        
        private static SVProgressHUD SharedView {
            get {
                if (sharedView==null) {
                    sharedView = new SVProgressHUD(UIScreen.MainScreen.Bounds);
                }
                return sharedView;
            }
        }
        
        private SVProgressHUDMask mask;
        
        private UIView hudView;
        private UIView HudView {
            get {
                if (hudView == null) {
                    hudView = new UIView();
                    hudView.Layer.CornerRadius = 10;
                    hudView.BackgroundColor =  UIColor.FromWhiteAlpha(0f, 0.8f);        
                    hudView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleTopMargin |
                                    UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin;
                    this.AddSubview(hudView);
                }
                return hudView;
            }
        }
        
        private UIImageView imageView;
        private UIImageView ImageView {
            get {
                if (imageView == null) {
                    imageView = new UIImageView(new RectangleF(0f, 0f, 28f, 28f));
                    HudView.AddSubview(imageView);
                }
                return imageView;
            }
        }
        
        private UIActivityIndicatorView spinnerView;
        private UIActivityIndicatorView SpinnerView {
            get {
                if (spinnerView == null) {
                    spinnerView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                    spinnerView.HidesWhenStopped = true;
                    spinnerView.Bounds = new RectangleF(0, 0, 37, 37);
                    this.HudView.AddSubview(spinnerView);
                }
                return spinnerView;
            }
        }
        
        private UIWindow overlayWindow;
        private UIWindow OverlayWindow {
            get {
                if (overlayWindow == null) {
                    overlayWindow = new UIWindow(UIScreen.MainScreen.Bounds);
                    overlayWindow.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    overlayWindow.BackgroundColor = UIColor.Clear;
                    overlayWindow.UserInteractionEnabled = false;
                }
                return overlayWindow;
            }
        }
        
        private SVProgressHUD() {
        }
        
        private SVProgressHUD (RectangleF frame): base(frame) {
            this.OverlayWindow.AddSubview(this);
            this.BackgroundColor = UIColor.Clear;
            this.Alpha = 0;
            this.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }
        
        public static void Show () {
            throw new NotImplementedException ();
        }

        public static void Show (string status) {
            throw new NotImplementedException ();
        }
        
        public static void Show (string status, SVProgressHUDMask mask) {
            throw new NotImplementedException ();
        }

        public static void Show (SVProgressHUDMask mask) {
            throw new NotImplementedException ();
        }
        
        public static void Dismiss () {
            throw new NotImplementedException ();
        }
        
        private void SetStatus(string status) {
        }
        
        private void ShowInternal(string status, SVProgressHUDMask mask) {
// self.fadeOutTimer = nil;
// 
//    if(self.showNetworkIndicator)
//        [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
//    
//    self.showNetworkIndicator = show;
//    
//    if(self.showNetworkIndicator)
//        [UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
            
            this.ImageView.Hidden = true;
            this.mask = mask;
            
            this.SetStatus(status);
            this.SpinnerView.StartAnimating();
            
            this.OverlayWindow.UserInteractionEnabled = (this.mask!=SVProgressHUDMask.None);
            this.OverlayWindow.MakeKeyAndVisible();
            
//    [self positionHUD:nil];
   
            if (this.Alpha != 1f) {
//        [self registerNotifications];
                this.HudView.Transform = CGAffineTransform.MakeScale(1.3f, 1.3f);
//     self.hudView.transform = CGAffineTransformScale(self.hudView.transform, 1.3, 1.3);
//     
                
                UIView.Animate(0.15f, 0,UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () => {
                    this.HudView.Transform =CGAffineTransform.MakeScale(1/1.3f, 1/1.3f);
                    this.Alpha = 1;
            }, () => {});    
//     [UIView animateWithDuration:0.15
//                           delay:0
//                         options:UIViewAnimationOptionAllowUserInteraction | UIViewAnimationCurveEaseOut | UIViewAnimationOptionBeginFromCurrentState
//                      animations:^{  
//                          self.hudView.transform = CGAffineTransformScale(self.hudView.transform, 1/1.3, 1/1.3);
//                             self.alpha = 1;
//                      }
//                      completion:NULL];
            }
//    
            this.SetNeedsDisplay();
        }
    }
}

