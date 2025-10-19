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
            Loaded += Home_Loaded;
        }

        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigureWeekendLayout();
            TryRenderUpcomingEvents();
            LoadCurrentAndNextClass();
            await LoadWeatherIfWeekendAsync();
        }

        private void ConfigureWeekendLayout()
        {
            var today = DateTime.Now.DayOfWeek;
            bool isWeekend = today == DayOfWeek.Saturday || today == DayOfWeek.Sunday;

            if (isWeekend)
            {
                // Hide current class section on weekends
                CurrentClassBorder.Visibility = Visibility.Collapsed;
                LeftColumn.Width = new GridLength(0);
                
                // Adjust greeting border margin to expand left
                GreetingBorder.Margin = new Thickness(0, 0, 20, 0);
                
                // Update center greeting for weekend
                MainGreeting.Text = today == DayOfWeek.Saturday ? "Happy Saturday!" : "Happy Sunday!";
                
                // Show weekend content in right section (weather)
                NextClassPanel.Visibility = Visibility.Collapsed;
                WeekendPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // Show normal weekday layout
                CurrentClassBorder.Visibility = Visibility.Visible;
                LeftColumn.Width = GridLength.Auto;
                
                // Normal greeting border margin
                GreetingBorder.Margin = new Thickness(20, 0, 20, 0);
                
                // Weekday greeting
                MainGreeting.Text = "Hey Kevin!";
                
                // Show next class content in right section
                NextClassPanel.Visibility = Visibility.Visible;
                WeekendPanel.Visibility = Visibility.Collapsed;
                
                // Reset to default gradient for weekdays
                SetWeatherGradient("Clear");
            }
        }

        private async Task LoadWeatherIfWeekendAsync()
        {
            var today = DateTime.Now.DayOfWeek;
            bool isWeekend = today == DayOfWeek.Saturday || today == DayOfWeek.Sunday;

            if (!isWeekend)
                return;

            try
            {
                var weather = await WeatherService.GetCurrentWeatherAsync();

                if (weather != null)
                {
                    WeatherTemperature.Text = $"{Math.Round(weather.Temperature)}°";
                    WeatherDescription.Text = char.ToUpper(weather.Description[0]) + weather.Description.Substring(1);
                    
                    // Update gradient based on weather condition
                    SetWeatherGradient(weather.Condition);
                }
                else
                {
                    WeatherTemperature.Text = "--°";
                    WeatherDescription.Text = "Unable to load weather";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load weather: {ex.Message}");
                WeatherTemperature.Text = "--°";
                WeatherDescription.Text = "Weather unavailable";
            }
        }

        private void SetWeatherGradient(string condition)
        {
            Color startColor, endColor;

            switch (condition.ToLower())
            {
                case "clear":
                    // Sunny gradient (yellow to orange)
                    startColor = (Color)ColorConverter.ConvertFromString("#F59E0B");
                    endColor = (Color)ColorConverter.ConvertFromString("#F97316");
                    break;

                case "clouds":
                    // Cloudy gradient (gray to blue-gray)
                    startColor = (Color)ColorConverter.ConvertFromString("#6B7280");
                    endColor = (Color)ColorConverter.ConvertFromString("#9CA3AF");
                    break;

                case "rain":
                case "drizzle":
                    // Rainy gradient (dark blue to blue)
                    startColor = (Color)ColorConverter.ConvertFromString("#1E40AF");
                    endColor = (Color)ColorConverter.ConvertFromString("#3B82F6");
                    break;

                case "snow":
                    // Snowy gradient (light blue to white-blue)
                    startColor = (Color)ColorConverter.ConvertFromString("#DBEAFE");
                    endColor = (Color)ColorConverter.ConvertFromString("#93C5FD");
                    break;

                case "thunderstorm":
                    // Storm gradient (dark purple to gray)
                    startColor = (Color)ColorConverter.ConvertFromString("#4C1D95");
                    endColor = (Color)ColorConverter.ConvertFromString("#6B7280");
                    break;

                default:
                    // Default gradient (original colors)
                    startColor = (Color)ColorConverter.ConvertFromString("#6366F1");
                    endColor = (Color)ColorConverter.ConvertFromString("#089DDA");
                    break;
            }

            RightSectionGradient.GradientStops[0].Color = startColor;
            RightSectionGradient.GradientStops[1].Color = endColor;
        }

        private void LoadCurrentAndNextClass()
        {
            try
            {
                var today = DateTime.Now.DayOfWeek;
                bool isWeekend = today == DayOfWeek.Saturday || today == DayOfWeek.Sunday;

                // Skip loading class info on weekends
                if (isWeekend)
                {
                    return;
                }

                var (current, next) = TimetableManager.GetCurrentAndNextClass();

                // Update current class
                if (current != null)
                {
                    CurrentClassSubject.Text = current.Subject;
                    // For breaks, show a dash instead of room
                    CurrentClassRoom.Text = current.IsBreak ? "-" : (string.IsNullOrWhiteSpace(current.Room) ? "-" : current.Room);
                    CurrentClassTime.Text = $"{current.StartTime} - {current.EndTime}";
                }
                else
                {
                    CurrentClassSubject.Text = "None";
                    CurrentClassRoom.Text = "-";
                    CurrentClassTime.Text = "-";
                }

                // Update next class
                if (next != null)
                {
                    NextClassSubject.Text = next.Subject;
                    // For breaks, show a dash instead of room
                    NextClassRoom.Text = next.IsBreak ? "-" : (string.IsNullOrWhiteSpace(next.Room) ? "-" : next.Room);
                    NextClassTime.Text = $"{next.StartTime} - {next.EndTime}";
                }
                else
                {
                    NextClassSubject.Text = "None";
                    NextClassRoom.Text = "-";
                    NextClassTime.Text = "-";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load class info: {ex.Message}");
            }
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
