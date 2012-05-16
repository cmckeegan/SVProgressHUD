using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

// MonoTouch Conversion of SVProgressHUD by Sam Vermette
//
//
// Original Licence
//
// Copyright (c) 2011 Sam Vermette
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// A different license may apply to other ressources included in this package, 
// including Joseph Wain's Glyphish Icons. Please consult their 
// respective headers for the terms of their individual licenses.

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
        
        private NSTimer fadeOutTimer;
        private SVProgressHUDMask mask;
        
        private UIView hudView;
        private UIView HudView {
            get {
                if (hudView == null) {
                    hudView = new UIView(RectangleF.Empty);
                    hudView.Layer.CornerRadius = 10;
                    hudView.BackgroundColor =  UIColor.FromWhiteAlpha(0f, 0.8f);        
                    hudView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleTopMargin |
                                    UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin;
                    this.AddSubview(hudView);
                }
                return hudView;
            }
        }
        
        private UILabel stringLabel;
        private UILabel StringLabel {
            get {
                if (stringLabel == null) {
                    stringLabel = new UILabel();
                    stringLabel.TextColor = UIColor.White;
                    stringLabel.BackgroundColor = UIColor.Clear;
                    stringLabel.AdjustsFontSizeToFitWidth = true;
                    stringLabel.TextAlignment = UITextAlignment.Center;
                    stringLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
                    stringLabel.Font = UIFont.BoldSystemFontOfSize(16f);
                    stringLabel.ShadowColor = UIColor.Black;
                    stringLabel.ShadowOffset = new SizeF(0, -1);
                    stringLabel.Lines = 0;
                    this.HudView.AddSubview(stringLabel);
                }
                
                return stringLabel;
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
        
        private SVProgressHUD (RectangleF frame): base(frame) {
            this.OverlayWindow.AddSubview(this);
            this.UserInteractionEnabled = false;
            this.BackgroundColor = UIColor.Clear;
            this.Alpha = 0;
            this.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }
        
        public override void Draw (RectangleF rect) {
            CGContext context = UIGraphics.GetCurrentContext();
            
            switch (this.mask) {
                case SVProgressHUDMask.Black : {
                    UIColor.FromWhiteAlpha(0f, 0.5f).SetColor();
                    context.FillRect(this.Bounds);
                    break;
                }
                case SVProgressHUDMask.Gradient: {
                    float[] locations = new float[2] {0.0f, 1.0f};
                    float[] colors = new float[8] {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.75f}; 
                    CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
                    CGGradient gradient = new CGGradient(colorSpace, colors, locations);
                    colorSpace.Dispose();
                
                    PointF center = new PointF(this.Bounds.Size.Width/2f, this.Bounds.Size.Height/2f);
                    float radius = Math.Min(this.Bounds.Size.Width, this.Bounds.Size.Height);
                    context.DrawRadialGradient(gradient, center, 0, center, radius, CGGradientDrawingOptions.DrawsAfterEndLocation);
                    gradient.Dispose();
                    break;
                }
            }
        }
        
        [Export("positionHUD:")]
        private void PositionHUD(NSNotification notification) {
            float keyboardHeight = 0;
            double animationDuration = 0;
            
            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            
            if (notification !=null) {
                if (notification.Name == UIKeyboard.WillShowNotification || notification.Name ==  UIKeyboard.DidShowNotification) {
                    
                    NSDictionary keyBoardInfo = notification.UserInfo;
                    RectangleF keyboardFrame = ((NSValue) keyBoardInfo[UIKeyboard.FrameBeginUserInfoKey]).RectangleFValue;
                    animationDuration = ((NSNumber)keyBoardInfo[UIKeyboard.AnimationDurationUserInfoKey]).DoubleValue;
                
                    if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown) 
                        keyboardHeight = keyboardFrame.Size.Height;
                    else
                        keyboardHeight = keyboardFrame.Size.Width;
                }
            }
            else {
                keyboardHeight = this.VisibleKeyboardHeight();
            }
            
            RectangleF orientationFrame = UIScreen.MainScreen.Bounds;
            RectangleF statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
            
            if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight) {
                orientationFrame = new RectangleF(orientationFrame.Location, new SizeF(orientationFrame.Size.Height, orientationFrame.Size.Width));
                statusBarFrame = new RectangleF(statusBarFrame.Location, new SizeF(statusBarFrame.Size.Height, statusBarFrame.Size.Width));
            }
            
            float activeHeight = orientationFrame.Size.Height;
            
            if (keyboardHeight > 0)
                activeHeight += statusBarFrame.Size.Height * 2;
            
            activeHeight -= keyboardHeight;
            float posY = (float)Math.Floor(activeHeight * 0.45f);
            float posX = orientationFrame.Size.Width / 2f;
         
            PointF newCenter;
            float rotateAngle;
            
            switch (orientation) {
                case UIInterfaceOrientation.PortraitUpsideDown:
                    rotateAngle = (float) Math.PI;
                    newCenter = new PointF(posX, orientationFrame.Size.Height - posY);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                    rotateAngle = - (float) Math.PI/2.0f;
                    newCenter = new PointF(posY, posX);
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    rotateAngle = (float) Math.PI/2.0f;
                    newCenter = new PointF(orientationFrame.Size.Height - posY, posX);
                    break;
                default:
                    rotateAngle = 0;
                    newCenter = new PointF(posX, posY);
                    break;
            }
            
            if (notification !=null) {
                UIView.Animate(animationDuration, 0, UIViewAnimationOptions.AllowUserInteraction, ()=>{
                    this.MoveToPoint(newCenter, rotateAngle);    
                }, 
                ()=>{}); 
            }
            else {
                MoveToPoint(newCenter, rotateAngle);
            }
        }
        
        private void MoveToPoint(PointF center, float angle) {
            this.HudView.Center = center;
            this.HudView.Transform = CGAffineTransform.MakeRotation(angle);
            this.HudView.Transform.Multiply(CGAffineTransform.MakeScale(1f,1f));
        }
        
        private float VisibleKeyboardHeight() {
//            NSAutoreleasePool *autoreleasePool = [[NSAutoreleasePool alloc] init];
            UIWindow keyboardWindow = null;
            
            foreach (var w in UIApplication.SharedApplication.Windows) {
                if (w.GetType() != typeof(UIWindow)) {
                    keyboardWindow = w;
                    break;
                }
            }
            
            if (keyboardWindow == null) return 0f;
            
            UIView foundKeyboard = null;
            foreach (UIView v in keyboardWindow.Subviews) {
                var possibleKeyboard = v;
//                // iOS 4 sticks the UIKeyboard inside a UIPeripheralHostView.
//                if ([[possibleKeyboard description] hasPrefix:@"<UIPeripheralHostView"]) {
//                    possibleKeyboard = [[possibleKeyboard subviews] objectAtIndex:0];
//                }  
                if (possibleKeyboard.ToString().Contains("UIPeripheralHostView")) {
                    possibleKeyboard = possibleKeyboard.Subviews[0];
                }
                                                          
//                
//                if ([[possibleKeyboard description] hasPrefix:@"<UIKeyboard"]) {
//                    foundKeyboard = possibleKeyboard;
//                    break;
//                }
                if (possibleKeyboard.ToString ().Contains("UIKeyboard")) {
                    foundKeyboard = possibleKeyboard;
                    break;
                }               
            }
//            [autoreleasePool release];
            if (foundKeyboard != null && foundKeyboard.Bounds.Size.Height > 100)
                return foundKeyboard.Bounds.Size.Height;
            
            return 0;
        }
        
        public static void Show () {
            Show(null, SVProgressHUDMask.None);
        }

        public static void Show (string status) {
            Show(status, SVProgressHUDMask.None);
        }
        
        public static void Show (string status, SVProgressHUDMask mask) {
            SharedView.ShowInternal(status, mask);
        }

        public static void Show (SVProgressHUDMask mask) {
            Show(null, mask);
        }
        
        private void RegisterNotifications() {
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("positionHUD:"), UIApplication.DidChangeStatusBarOrientationNotification, null);
//    [[NSNotificationCenter defaultCenter] addObserver:self 
//                                             selector:@selector(positionHUD:) 
//                                                 name:UIApplicationDidChangeStatusBarOrientationNotification 
//                                               object:nil];  
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("positionHUD:"), UIKeyboard.WillHideNotification, null);
//    [[NSNotificationCenter defaultCenter] addObserver:self 
//                                             selector:@selector(positionHUD:) 
//                                                 name:UIKeyboardWillHideNotification
//                                               object:nil];
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("positionHUD:"), UIKeyboard.DidHideNotification, null);
//    [[NSNotificationCenter defaultCenter] addObserver:self 
//                                             selector:@selector(positionHUD:) 
//                                                 name:UIKeyboardDidHideNotification
//                                               object:nil];
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("positionHUD:"), UIKeyboard.WillShowNotification, null);
//    [[NSNotificationCenter defaultCenter] addObserver:self 
//                                             selector:@selector(positionHUD:) 
//                                                 name:UIKeyboardWillShowNotification
//                                               object:nil];
            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("positionHUD:"), UIKeyboard.DidShowNotification, null);
//    [[NSNotificationCenter defaultCenter] addObserver:self 
//                                             selector:@selector(positionHUD:) 
//                                                 name:UIKeyboardDidShowNotification
//                                               object:nil];
        }
        
        public static void Dismiss () {
            SharedView.DismissInternal();
        }
        
        public static void DismissWithSuccess(string text) {
            DismissWithSuccess(text, 0.9f);    
        }
        
        public static void DismissWithSuccess(string text, float delay) {
            SharedView.DismissWithStatus(text, false, delay);  
        }
        
        public static void DismissWithError(string text) {
            DismissWithError(text, 0.9f);    
        }
        
        public static void DismissWithError(string text, float delay) {
            SharedView.DismissWithStatus(text, true, delay);  
        }
    
        private void SetStatus(string status) {
            
            float hudWidth = 100;
            float hudHeight = 100;
            RectangleF labelRect = RectangleF.Empty;
            
            if (status != null) {
                SizeF stringSize = new NSString(status).StringSize(this.StringLabel.Font, new SizeF(200f, 300f));
                hudHeight = 80 + stringSize.Height;
                
                if (stringSize.Width > hudWidth)
                    hudWidth = (float) Math.Ceiling(stringSize.Width/2f) * 2f;
                
                if (hudHeight > 100) {
                    labelRect = new RectangleF(12, 66, hudWidth, stringSize.Height);
                    hudWidth += 24;
                }
                else {
                    hudWidth += 24;
                    labelRect = new RectangleF(0, 66, hudWidth, stringSize.Height);
                }
            }
            
            this.HudView.Bounds = new RectangleF(0, 0, hudWidth, hudHeight);
            
            if (status!=null) {
                this.ImageView.Center = new PointF(this.HudView.Bounds.Width/2, 36);
                this.SpinnerView.Center = new PointF((float) Math.Ceiling(this.HudView.Bounds.Width/2) + 0.5f, 40.5f);
            }
            else {
                this.ImageView.Center = new PointF(this.HudView.Bounds.Width/2, this.HudView.Bounds.Height / 2);
                this.SpinnerView.Center = new PointF((float) Math.Ceiling(this.HudView.Bounds.Width/2f) + 0.5f, (float)Math.Ceiling(this.HudView.Bounds.Height/2f) + 0.5f);
            }
            
            this.StringLabel.Hidden = false;
            this.StringLabel.Text = status;
            this.StringLabel.Frame = labelRect;
        }
        
        
        private NSTimer FadeOutTimer {
            get {
                return fadeOutTimer;
            }
            set {
                if (fadeOutTimer !=null) {
                    fadeOutTimer.Invalidate();
                    fadeOutTimer.Dispose();
                }
                
                fadeOutTimer = value;
            }
        }
        
        private void ShowInternal(string status, SVProgressHUDMask mask) {
            this.FadeOutTimer = null;
            
            this.ImageView.Hidden = true;
            this.mask = mask;
            
            this.SetStatus(status);
            this.SpinnerView.StartAnimating();
            
            this.OverlayWindow.UserInteractionEnabled = (this.mask!=SVProgressHUDMask.None);
            this.OverlayWindow.MakeKeyAndVisible();
            
            this.PositionHUD(null);
   
            if (this.Alpha != 1f) {
                RegisterNotifications();
                var transform = this.HudView.Transform;
                transform.Scale(1.3f,1.3f);
                this.HudView.Transform = transform;
                
                UIView.Animate(0.15f, 0, UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () => {
                    
                    var normal = this.HudView.Transform;
                    normal.Scale(1/1.3f,1/1.3f);
                    this.HudView.Transform = normal;
                    this.Alpha = 1;
            }, 
            () => {});    
            }

            
            this.SetNeedsDisplay();
        }
    
        private void DismissInternal() {
            UIView.Animate(0.15f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.AllowUserInteraction,
                           ()=>{
                
                                var small = this.HudView.Transform;
                                small.Scale(0.8f, 0.8f);
                                SharedView.HudView.Transform = small;
                                SharedView.Alpha = 0;
                            }, 
                            ()=>{
                                if (SharedView.Alpha == 0) {
                                    NSNotificationCenter.DefaultCenter.RemoveObserver(SharedView);
                                    overlayWindow.Dispose();
                                    overlayWindow = null;
                                    sharedView.Dispose();
                                    sharedView = null;
                                }
                                foreach (UIWindow w in UIApplication.SharedApplication.Windows) {
                                    if (w.WindowLevel == UIWindow.LevelNormal) {
                                        w.MakeKeyWindow();
                                        break;
                                    }    
                                }
                            });
        }
        
        private void DismissWithStatus(string text, bool error, float delay) {
            if (this.Alpha != 1) return;
            
            this.ImageView.Image = UIImage.FromBundle(error ? @"SVProgressHUD.bundle/error.png" : @"SVProgressHUD.bundle/success.png");
            this.ImageView.Hidden = false;
            this.SetStatus(text);
            this.SpinnerView.StopAnimating();
            
            this.FadeOutTimer = NSTimer.CreateScheduledTimer(delay, ()=>DismissInternal());
        }
    }
}

