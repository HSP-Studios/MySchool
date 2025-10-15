using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MySchool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindow_StateChanged;
            this.MinWidth = 1000;
            this.MinHeight = 400;
            this.MaxWidth = 1000;
            this.MaxHeight = 400;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            // Update maximize/restore icon based on state
            var isMax = WindowState == WindowState.Maximized;
            var maxIcon = this.FindName("MaxIcon") as TextBlock;
            if (maxIcon != null)
            {
                // E922 = Maximize, E923 = Restore
                maxIcon.Text = isMax ? "\uE923" : "\uE922";
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowView(UIElement show, params UIElement[] hide)
        {
            show.Visibility = Visibility.Visible;
            foreach (var h in hide)
                h.Visibility = Visibility.Collapsed;
        }

        private void SetTabHighlight(string selected)
        {
            var blue = (Brush)FindResource("Brush.Primary");
            var gray = (Brush)FindResource("Brush.TextSecondary");
            if (ScheduleTabIcon != null) ScheduleTabIcon.Foreground = selected == "Schedule" ? blue : gray;
            if (HomeTabIcon != null) HomeTabIcon.Foreground = selected == "Home" ? blue : gray;
            if (SettingsTabIcon != null) SettingsTabIcon.Foreground = selected == "Settings" ? blue : gray;
        }

        private void HomeTab_Click(object sender, RoutedEventArgs e)
        {
            if (HomeView != null && ScheduleView != null && SettingsView != null)
                ShowView(HomeView, ScheduleView, SettingsView);
            SetTabHighlight("Home");
        }

        private void ScheduleTab_Click(object sender, RoutedEventArgs e)
        {
            if (HomeView != null && ScheduleView != null && SettingsView != null)
                ShowView(ScheduleView, HomeView, SettingsView);
            SetTabHighlight("Schedule");
        }

        private void SettingsTab_Click(object sender, RoutedEventArgs e)
        {
            if (HomeView != null && ScheduleView != null && SettingsView != null)
                ShowView(SettingsView, HomeView, ScheduleView);
            SetTabHighlight("Settings");
        }

        // Set initial tab highlight to Home on load
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SetTabHighlight("Home");
        }

        private void AppBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaxRestoreButton_Click(sender, e);
                return;
            }
            try
            {
                DragMove();
            }
            catch { /* ignore */ }
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            var dpi = VisualTreeHelper.GetDpi(this);
            int minWidthPx = (int)Math.Ceiling(this.MinWidth * dpi.DpiScaleX);
            int minHeightPx = (int)Math.Ceiling(this.MinHeight * dpi.DpiScaleY);
            int maxWidthPx = (int)Math.Ceiling(this.MaxWidth * dpi.DpiScaleX);
            int maxHeightPx = (int)Math.Ceiling(this.MaxHeight * dpi.DpiScaleY);

            // Get the primary monitor information for the window
            IntPtr monitor = MonitorFromWindow(hwnd, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                if (GetMonitorInfo(monitor, ref monitorInfo))
                {
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;

                    // If maximized, allow full screen size
                    if (WindowState == WindowState.Maximized)
                    {
                        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                    }
                    else
                    {
                        // Otherwise, lock to 1000x400
                        mmi.ptMaxSize.x = maxWidthPx;
                        mmi.ptMaxSize.y = maxHeightPx;
                        mmi.ptMinTrackSize.x = minWidthPx;
                        mmi.ptMinTrackSize.y = minHeightPx;
                    }
                }
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}