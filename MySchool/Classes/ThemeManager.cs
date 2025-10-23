using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MySchool.Classes
{
    public static class ThemeManager
    {
        private static Image? _transitionOverlay;
        private static bool _isTransitioning;

        private static Color Lighten(Color color, double amount)
        {
            if (amount < 0) amount = 0;
            if (amount > 1) amount = 1;
            byte r = (byte)(color.R + (255 - color.R) * amount);
            byte g = (byte)(color.G + (255 - color.G) * amount);
            byte b = (byte)(color.B + (255 - color.B) * amount);
            return Color.FromArgb(255, r, g, b);
        }

        private static string _currentTheme = "Light";

        public static string CurrentTheme
        {
            get => _currentTheme;
            private set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    OnPropertyChanged(nameof(CurrentTheme));
                }
            }
        }

        public static event PropertyChangedEventHandler? PropertyChanged;

        private static void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static void ApplyTheme(bool dark)
        {
            CurrentTheme = dark ? "Dark" : "Light";

            var resources = Application.Current.Resources;

            if (dark)
            {
                resources["Brush.Primary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#818CF8"));
                resources["Brush.PrimaryDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
                resources["Brush.Accent"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22D3EE"));
                // Background and card surface (card slightly lighter than background)
                var bg = (Color)ColorConverter.ConvertFromString("#0B1220");
                resources["Brush.Background"] = new SolidColorBrush(bg);
                var surface = Lighten(bg, 0.10);
                resources["Brush.Surface"] = new SolidColorBrush(surface);
                // Lighter text for contrast
                resources["Brush.TextPrimary"] = new SolidColorBrush(Colors.White);
                resources["Brush.TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7D2FE"));
                // Border color for dark mode
                resources["Brush.Border"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B5563"));
            }
            else
            {
                resources["Brush.Primary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4F46E5"));
                resources["Brush.PrimaryDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4338CA"));
                resources["Brush.Accent"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#06B6D4"));
                // Slightly darker neutral background so surfaces (white) stand out
                var bg = (Color)ColorConverter.ConvertFromString("#EEF2F7");
                resources["Brush.Background"] = new SolidColorBrush(bg);
                // Use pure white for surfaces (cards/app bar) for clear separation
                resources["Brush.Surface"] = new SolidColorBrush(Colors.White);
                resources["Brush.TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                resources["Brush.TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));
                // Border color for light mode
                resources["Brush.Border"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));
            }
        }

        /// <summary>
        /// Applies the theme with a cross-fade transition over the specified duration.
        /// </summary>
        /// <param name="dark">True for dark mode.</param>
        /// <param name="durationSeconds">Length of the fade in seconds. Default 0.5s.</param>
        public static void ApplyThemeWithTransition(bool dark, double durationSeconds = 0.5)
        {
            var window = Application.Current?.MainWindow;
            if (window == null || durationSeconds <= 0)
            {
                ApplyTheme(dark);
                return;
            }

            if (_isTransitioning)
            {
                // Best-effort cleanup of a prior transition
                if (_transitionOverlay != null)
                {
                    var parent = _transitionOverlay.Parent as Panel;
                    parent?.Children.Remove(_transitionOverlay);
                }
                _transitionOverlay = null;
                _isTransitioning = false;
            }

            // We need a Panel to inject an overlay Image.
            if (window.Content is not FrameworkElement rootElement)
            {
                // Fallback: no transition possible on unknown visual tree.
                ApplyTheme(dark);
                return;
            }
            if (window.Content is not Panel rootPanel)
            {
                // Fallback: no transition possible on unknown visual tree.
                ApplyTheme(dark);
                return;
            }

            // Ensure layout is current and dimensions are valid
            rootElement.UpdateLayout();
            if (rootElement.ActualWidth <= 0 || rootElement.ActualHeight <= 0)
            {
                ApplyTheme(dark);
                return;
            }

            // Capture current visual into a bitmap snapshot
            var dpi = VisualTreeHelper.GetDpi(rootElement);
            int pixelWidth = (int)Math.Max(1, Math.Round(rootElement.ActualWidth * dpi.DpiScaleX));
            int pixelHeight = (int)Math.Max(1, Math.Round(rootElement.ActualHeight * dpi.DpiScaleY));
            var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            rtb.Render(rootElement);

            // Create overlay Image with the snapshot on top of the UI
            var overlay = new Image
            {
                Source = rtb,
                Stretch = Stretch.Fill,
                Opacity = 1.0,
                IsHitTestVisible = false,
            };
            Panel.SetZIndex(overlay, int.MaxValue);

            // Try to span all rows/columns if it's a Grid
            if (rootPanel is Grid grid)
            {
                Grid.SetRow(overlay, 0);
                Grid.SetColumn(overlay, 0);
                Grid.SetRowSpan(overlay, Math.Max(1, grid.RowDefinitions.Count));
                Grid.SetColumnSpan(overlay, Math.Max(1, grid.ColumnDefinitions.Count));
            }

            rootPanel.Children.Add(overlay);

            // Apply the new theme under the overlay
            ApplyTheme(dark);

            _transitionOverlay = overlay;
            _isTransitioning = true;

            // Animate the overlay opacity from 1 -> 0
            var duration = TimeSpan.FromSeconds(durationSeconds);
            var anim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(duration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            anim.Completed += (s, e) =>
            {
                // Clean up
                if (_transitionOverlay != null)
                {
                    var parent = _transitionOverlay.Parent as Panel;
                    parent?.Children.Remove(_transitionOverlay);
                }
                _transitionOverlay = null;
                _isTransitioning = false;
            };

            overlay.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
