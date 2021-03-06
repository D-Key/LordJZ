﻿using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using LordJZ.WinAPI;
using LordJZ.WinAPI.Native;

namespace LordJZ.Presentation.Controls
{
    /// <remarks>
    /// Partially taken from MahApps.Metro.
    /// </remarks>
    [TemplatePart(Name = PART_TitleBar, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Border, Type = typeof(Border))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MaximizeRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MinimizeButton, Type = typeof(Button))]
    public class BaseWindow : Window
    {
        const string PART_TitleBar = "PART_TitleBar";
        const string PART_Border = "PART_Border";
        const string PART_CloseButton = "PART_CloseButton";
        const string PART_MaximizeRestoreButton = "PART_MaximizeRestoreButton";
        const string PART_MinimizeButton = "PART_MinimizeButton";

        #region Dependency Properties

        #region TitleBar Buttons properties

        #region Min Button

        public static readonly DependencyProperty ShowMinButtonProperty =
            DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(BaseWindow),
                                        new PropertyMetadata(BooleanBoxes.True));

        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, BooleanBoxes.Box(value)); }
        }

        public static readonly DependencyProperty MinButtonToolTipProperty =
            DependencyProperty.Register("MinButtonToolTip", typeof(string), typeof(BaseWindow));

        public string MinButtonToolTip
        {
            get { return (string)GetValue(MinButtonToolTipProperty); }
            set { SetValue(MinButtonToolTipProperty, value); }
        }

        #endregion

        #region MaxRestore Button

        public static readonly DependencyProperty ShowMaxRestoreButtonProperty =
            DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(BaseWindow),
                                        new PropertyMetadata(BooleanBoxes.True));

        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, BooleanBoxes.Box(value)); }
        }

        public static readonly DependencyProperty MaxButtonToolTipProperty =
            DependencyProperty.Register("MaxButtonToolTip", typeof(string), typeof(BaseWindow));

        public string MaxButtonToolTip
        {
            get { return (string)GetValue(MaxButtonToolTipProperty); }
            set { SetValue(MaxButtonToolTipProperty, value); }
        }

        public static readonly DependencyProperty RestoreButtonToolTipProperty =
            DependencyProperty.Register("RestoreButtonToolTip", typeof(string), typeof(BaseWindow));

        public string RestoreButtonToolTip
        {
            get { return (string)GetValue(RestoreButtonToolTipProperty); }
            set { SetValue(RestoreButtonToolTipProperty, value); }
        }

        #endregion

        #region Close Button

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(BaseWindow),
                                        new PropertyMetadata(BooleanBoxes.True));

        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value)); }
        }

        public static readonly DependencyProperty CloseButtonToolTipProperty =
            DependencyProperty.Register("CloseButtonToolTip", typeof(string), typeof(BaseWindow));

        public string CloseButtonToolTip
        {
            get { return (string)GetValue(CloseButtonToolTipProperty); }
            set { SetValue(CloseButtonToolTipProperty, value); }
        }

        #endregion

        #endregion

        #region TitleBar properties

        #region TitleBarHeight

        public double TitleBarHeight
        {
            get { return (double)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(BaseWindow),
                                        new PropertyMetadata((double)24));

        #endregion

        #region TitleBarBackground

        public Brush TitleBarBackground
        {
            get { return (Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(BaseWindow),
                                        new PropertyMetadata(null));

        #endregion

        #region TitleBarContent

        public object TitleBarContent
        {
            get { return GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }

        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(object), typeof(BaseWindow),
                                        new PropertyMetadata(OnTitleBarContentChanged));

        static void OnTitleBarContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (BaseWindow)d;
            window.HasTitleBarContent = e.NewValue != null;
        }

        #endregion

        #region HasTitleBarContent

        static readonly DependencyPropertyKey HasTitleBarContentPropertyKey =
            DependencyProperty.RegisterReadOnly("HasTitleBarContent", typeof(bool), typeof(BaseWindow),
                                                new PropertyMetadata(BooleanBoxes.False));

        public static readonly DependencyProperty HasTitleBarContentProperty =
            HasTitleBarContentPropertyKey.DependencyProperty;

        [ReadOnly(true)]
        public bool HasTitleBarContent
        {
            get { return (bool)GetValue(HasTitleBarContentProperty); }
            private set { SetValue(HasTitleBarContentPropertyKey, BooleanBoxes.Box(value)); }
        }

        #endregion

        #endregion

        #region IgnoreTaskbarOnMaximize

        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskBar", typeof(bool), typeof(BaseWindow), new PropertyMetadata(BooleanBoxes.False));

        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { SetValue(IgnoreTaskbarOnMaximizeProperty, value); }
        }

        #endregion

        #endregion

        #region Properties

        IntPtr m_handle;

        internal IntPtr Handle
        {
            get
            {
                if (m_handle != IntPtr.Zero)
                    return m_handle;

                return m_handle = this.GetWin32Handle();
            }
        }

        #endregion

        #region Constructors

        static BaseWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseWindow),
                                                     new FrameworkPropertyMetadata(typeof(BaseWindow)));
            WindowStyleProperty.OverrideMetadata(typeof(BaseWindow),
                                                 new FrameworkPropertyMetadata(WindowStyle.None));
        }

        public BaseWindow()
        {
            //var baseStyle = Application.Current.FindResource(typeof(BaseWindow)) as Style;
            //this.Style = new Style(GetType(), baseStyle);
        }

        #endregion

        #region Overrides

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (this.WindowState == WindowState.Maximized)
            {
                IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(this.Handle, Constants.MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    var monitorInfo = new MONITORINFO();
                    UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                    bool ignoreTaskBar = this.IgnoreTaskbarOnMaximize;
                    var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                    var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                    var cx = ignoreTaskBar ? monitorInfo.rcWork.right : Math.Abs(monitorInfo.rcWork.right - x);
                    var cy = ignoreTaskBar ? monitorInfo.rcMonitor.bottom : Math.Abs(monitorInfo.rcWork.bottom - y);
                    UnsafeNativeMethods.SetWindowPos(this.Handle, new IntPtr(-2), x, y, cx, cy, 0x0040);
                }
            }
        }

        FrameworkElement m_titleBar;
        Border m_border;
        Button m_closeButton;
        Button m_maximizeRestoreButton;
        Button m_minimizeButton;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            #region TitleBar itself

            if (m_titleBar != null)
            {
                m_titleBar.MouseDown -= this.TitleBarMouseDown;
                m_titleBar.MouseUp -= this.TitleBarMouseUp;
                m_titleBar.MouseMove -= this.TitleBarMouseMove;
            }
            else
                MouseDown -= this.TitleBarMouseDown;

            m_titleBar = GetTemplateChild(PART_TitleBar) as FrameworkElement;

            if (m_titleBar != null)
            {
                m_titleBar.MouseDown += this.TitleBarMouseDown;
                m_titleBar.MouseUp += this.TitleBarMouseUp;
                m_titleBar.MouseMove += this.TitleBarMouseMove;
            }
            else
                MouseDown += this.TitleBarMouseDown;

            #endregion

            #region TitleBar buttons

            if (m_closeButton != null)
                m_closeButton.Click -= this.CloseButton_Click;

            if (m_maximizeRestoreButton != null)
                m_maximizeRestoreButton.Click -= this.MaximizeRestoreButton_Click;

            if (m_minimizeButton != null)
                m_minimizeButton.Click -= this.MinimizeButton_Click;

            m_closeButton = GetTemplateChild(PART_CloseButton) as Button;
            m_maximizeRestoreButton = GetTemplateChild(PART_MaximizeRestoreButton) as Button;
            m_minimizeButton = GetTemplateChild(PART_MinimizeButton) as Button;

            if (m_closeButton != null)
                m_closeButton.Click += this.CloseButton_Click;

            if (m_maximizeRestoreButton != null)
                m_maximizeRestoreButton.Click += this.MaximizeRestoreButton_Click;

            if (m_minimizeButton != null)
                m_minimizeButton.Click += this.MinimizeButton_Click;

            #endregion

            m_border = this.GetTemplateChild(PART_Border) as Border;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.SetNonClientRenderingPolicy(NonClientRenderingPolicy.Enabled);
            this.ExtendFrameIntoClientArea(new Thickness(1));

            // set the default background color of the window -> this avoids the black stripes when resizing
            this.SetDefaultBackgroundColor();

            var source = this.GetHwndSource();
            Contract.Assert(source != null);
            source.AddHook(HwndSourceHook);
        }

        void SetDefaultBackgroundColor()
        {
            var bgSolidColorBrush = this.Background as SolidColorBrush;

            if (bgSolidColorBrush != null)
                this.SetDefaultBackgroundColor(bgSolidColorBrush.Color);
        }

        #endregion

        #region Title Bar

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState != WindowState.Normal ? WindowState.Normal : WindowState.Maximized;
        }

        void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If left mouse button down but not right ...
            if (e.ChangedButton == MouseButton.Left && e.RightButton != MouseButtonState.Pressed)
            {
                // ... on the icon ...
                var mousePosition = GetCorrectPosition(this);
                var titleBarHeight = m_titleBar.ActualHeight;
                if (mousePosition.X <= titleBarHeight && mousePosition.Y <= titleBarHeight)
                {
                    // ... close if double click.
                    if (e.ClickCount >= 2)
                    {
                        Close();
                        return;
                    }

                    // ... or show the menu.
                    ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(0, titleBarHeight)));
                }
                // ... double click on the title bar ...
                else if (e.ClickCount >= 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize))
                    // ... maximize/restore the window.
                    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                // ... single click on the title bar ...
                else
                    // ... drag the window.
                    DragMove();
            }
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Right-click on the title bar without left button down should open the system menu
            if (e.ChangedButton == MouseButton.Right && e.LeftButton != MouseButtonState.Pressed)
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(GetCorrectPosition(this)));
        }

        private static Point GetCorrectPosition(Visual relativeTo)
        {
            UnsafeNativeMethods.Win32Point w32Mouse;
            UnsafeNativeMethods.GetCursorPos(out w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Maximized
                && ResizeMode != ResizeMode.NoResize)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                Left = left;

                // Restore window to normal state.
                WindowState = WindowState.Normal;

                DragMove();
            }
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }

        #endregion

        #region WndProc

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        IntPtr HwndSourceHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr returnval = IntPtr.Zero;
            switch (message)
            {
                case Constants.WM_NCCALCSIZE:
                    handled = true;
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = UnsafeNativeMethods.DefWindowProc(this.Handle, message, wParam, new IntPtr(-1));

                        if (m_border != null)
                            m_border.Opacity = wParam == IntPtr.Zero ? 1.0 : 0.5;

                        handled = true;
                    }
                    break;
                case Constants.WM_GETMINMAXINFO:
                    /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(this.Handle, lParam);

                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
                case Constants.WM_NCHITTEST:

                    // don't process the message on windows that can't be resized
                    var resizeMode = this.ResizeMode;
                    if (resizeMode == ResizeMode.CanMinimize || resizeMode == ResizeMode.NoResize || this.WindowState == WindowState.Maximized)
                        break;

                    // get X & Y out of the message                   
                    var screenPoint = new Point(UnsafeNativeMethods.GET_X_LPARAM(lParam), UnsafeNativeMethods.GET_Y_LPARAM(lParam));

                    // convert to window coordinates
                    var windowPoint = this.PointFromScreen(screenPoint);
                    var windowSize = this.RenderSize;
                    var windowRect = new Rect(windowSize);
                    windowRect.Inflate(-6, -6);

                    // don't process the message if the mouse is outside the 6px resize border
                    if (windowRect.Contains(windowPoint))
                        break;

                    var windowHeight = (int)windowSize.Height;
                    var windowWidth = (int)windowSize.Width;

                    // create the rectangles where resize arrows are shown
                    var topLeft = new Rect(0, 0, 6, 6);
                    var top = new Rect(6, 0, windowWidth - 12, 6);
                    var topRight = new Rect(windowWidth - 6, 0, 6, 6);

                    var left = new Rect(0, 6, 6, windowHeight - 12);
                    var right = new Rect(windowWidth - 6, 6, 6, windowHeight - 12);

                    var bottomLeft = new Rect(0, windowHeight - 6, 6, 6);
                    var bottom = new Rect(6, windowHeight - 6, windowWidth - 12, 6);
                    var bottomRight = new Rect(windowWidth - 6, windowHeight - 6, 6, 6);

                    // check if the mouse is within one of the rectangles
                    if (topLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPLEFT;
                    else if (top.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOP;
                    else if (topRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPRIGHT;
                    else if (left.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTLEFT;
                    else if (right.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTRIGHT;
                    else if (bottomLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMLEFT;
                    else if (bottom.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOM;
                    else if (bottomRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMRIGHT;

                    if (returnval != IntPtr.Zero)
                        handled = true;

                    break;

                case Constants.WM_INITMENU:
                    var hWnd = this.Handle;

                    if (!this.ShowMaxRestoreButton)
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                    else if (this.WindowState == WindowState.Maximized)
                    {
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                    }
                    else
                    {
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                    }

                    if (!this.ShowMinButton)
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MINIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);

                    if (this.ResizeMode == ResizeMode.NoResize || this.WindowState == WindowState.Maximized)
                        UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_SIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                    break;
            }

            return returnval;
        }

        #endregion
    }
}
