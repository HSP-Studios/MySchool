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
            this.MinWidth = 928;
            this.MinHeight = 632;
            this.MaxWidth = 928;
            this.MaxHeight = 632;
            this.ResizeMode = ResizeMode.NoResize;
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
            try
            {
                DragMove();
            }
            catch { /* ignore */ }
        }
    }
}