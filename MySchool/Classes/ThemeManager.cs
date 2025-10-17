using System.Windows;
using System.Windows.Media;

namespace MySchool
{
    public static class ThemeManager
    {
        private static Color Lighten(Color color, double amount)
        {
            if (amount < 0) amount = 0;
            if (amount > 1) amount = 1;
            byte r = (byte)(color.R + (255 - color.R) * amount);
            byte g = (byte)(color.G + (255 - color.G) * amount);
            byte b = (byte)(color.B + (255 - color.B) * amount);
            return Color.FromArgb(255, r, g, b);
        }

        public static void ApplyTheme(bool dark)
        {
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
            }
            else
            {
                resources["Brush.Primary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4F46E5"));
                resources["Brush.PrimaryDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4338CA"));
                resources["Brush.Accent"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#06B6D4"));
                var bg = (Color)ColorConverter.ConvertFromString("#F5F7FB");
                resources["Brush.Background"] = new SolidColorBrush(bg);
                var surface = Lighten(bg, 0.12);
                resources["Brush.Surface"] = new SolidColorBrush(surface);
                resources["Brush.TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                resources["Brush.TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));
            }
        }
    }
}
