using MySchool.Classes;
using MySchool.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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
            this.MinHeight = 600;
            this.MaxWidth = 928;
            this.MaxHeight = 600;
            this.ResizeMode = ResizeMode.NoResize;
            // Navigate to Home on startup
            ContentFrame.Navigate(new Home());
            SetTabHighlight("Home");
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
            if (ContentFrame != null && ContentFrame.Content?.GetType() != typeof(Home))
            {
                PageTransition.AnimatePageTransition(ContentFrame, new Home(), 0.3);
            }
            SetTabHighlight("Home");
        }

        private void ScheduleTab_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame != null && ContentFrame.Content?.GetType() != typeof(Schedule))
            {
                PageTransition.AnimatePageTransition(ContentFrame, new Schedule(), 0.3);
            }
            SetTabHighlight("Schedule");
        }

        private void SettingsTab_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame != null && ContentFrame.Content?.GetType() != typeof(Settings))
            {
                PageTransition.AnimatePageTransition(ContentFrame, new Settings(), 0.3);
            }
            SetTabHighlight("Settings");
        }

        // Set initial tab highlight to Home on load
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            // already set in constructor
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