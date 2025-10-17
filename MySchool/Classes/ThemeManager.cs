using System.Windows;
using System.Windows.Media;

namespace MySchool
{
    public static class ThemeManager
    {
        public static void ApplyTheme(bool dark)
        {
            var resources = Application.Current.Resources;

            if (dark)
            {
                resources["Brush.Primary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#818CF8"));
                resources["Brush.PrimaryDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
                resources["Brush.Accent"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22D3EE"));
                resources["Brush.Surface"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
                resources["Brush.Background"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B1220"));
                resources["Brush.TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB"));
                resources["Brush.TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
            else
            {
                resources["Brush.Primary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4F46E5"));
                resources["Brush.PrimaryDark"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4338CA"));
                resources["Brush.Accent"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#06B6D4"));
                resources["Brush.Surface"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                resources["Brush.Background"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F7FB"));
                resources["Brush.TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                resources["Brush.TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));
            }
        }
    }
}
