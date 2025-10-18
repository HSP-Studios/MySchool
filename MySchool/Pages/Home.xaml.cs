using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySchool.Classes;

namespace MySchool.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            TryRenderUpcomingEvents();
        }

        private void TryRenderUpcomingEvents()
        {
            try
            {
                var events = HolidayLogic.GetUpcomingEvents(3);
                if (UpcomingEventsPanel == null) return;

                UpcomingEventsPanel.Children.Clear();
                if (events == null || events.Count == 0)
                {
                    var tb = new TextBlock { Text = "No upcoming events", Opacity = 0.8 };
                    tb.SetResourceReference(TextBlock.StyleProperty, "Text.Body");
                    UpcomingEventsPanel.Children.Add(tb);
                    return;
                }

                foreach (var e in events)
                {
                    var row = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 0, 10) };

                    var ellipse = new Ellipse { Width = 10, Height = 10, VerticalAlignment = VerticalAlignment.Center };
                    // Color by event kind to match original design accents
                    switch (e.Kind)
                    {
                        case HolidayLogic.EventKind.TermStart:
                            ellipse.Fill = (Brush)FindResource("Brush.Primary");
                            break;
                        case HolidayLogic.EventKind.TermEnd:
                            ellipse.Fill = (Brush)FindResource("Brush.Accent");
                            break;
                        case HolidayLogic.EventKind.SchoolHoliday:
                            ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")); // teal/green
                            break;
                        default:
                            ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                            break;
                    }

                    var textStack = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
                    var title = new TextBlock { FontWeight = FontWeights.SemiBold, Text = e.Title };
                    var date = new TextBlock { };
                    date.SetResourceReference(TextBlock.StyleProperty, "Text.Body");
                    date.FontSize = 14;
                    date.Text = HolidayLogic.FormatDate(e.Date, e.EndDate);

                    textStack.Children.Add(title);
                    textStack.Children.Add(date);

                    row.Children.Add(ellipse);
                    row.Children.Add(textStack);

                    UpcomingEventsPanel.Children.Add(row);
                }
            }
            catch (Exception ex)
            {
                // Log the exception to help with debugging, but avoid breaking UI
                System.Diagnostics.Debug.WriteLine($"Exception in TryRenderUpcomingEvents: {ex}");
            }
        }
    }
}
