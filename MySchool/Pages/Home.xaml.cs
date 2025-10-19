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
        private static bool hasLoadedWeather = false;
        private static WeatherData? cachedWeather = null;

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
            
            // Load weather once or display cached data
            if (!hasLoadedWeather)
            {
                await LoadWeatherIfWeekendAsync();
                hasLoadedWeather = true;
            }
            else if (cachedWeather != null)
            {
                // Display cached weather data
                DisplayWeatherData(cachedWeather);
            }
        }

        private string GetWeekdayGreeting()
        {
            string userName = App.CurrentSettings.UserName;
            
            if (!string.IsNullOrWhiteSpace(userName))
            {
                return $"Hey {userName}!";
            }
            
            // Use time-based greeting if no name is set
            var hour = DateTime.Now.Hour;
            
            if (hour >= 5 && hour < 12)
            {
                return "Good Morning!";
            }
            else if (hour >= 12 && hour < 17)
            {
                return "Good Afternoon!";
            }
            else
            {
                return "Good Evening!";
            }
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
                MainGreeting.Text = GetWeekdayGreeting();
                
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
                    cachedWeather = weather;
                    DisplayWeatherData(weather);
                }
                else
                {
                    WeatherTemperature.Text = "--°";
                    WeatherDescription.Text = "Unable to load weather";
                    WeatherLocationLabel.Text = "Weather";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load weather: {ex.Message}");
                WeatherTemperature.Text = "--°";
                WeatherDescription.Text = "Weather unavailable";
                WeatherLocationLabel.Text = "Weather";
            }
        }

        private void DisplayWeatherData(WeatherData weather)
        {
            WeatherTemperature.Text = $"{Math.Round(weather.Temperature)}°";
            WeatherDescription.Text = char.ToUpper(weather.Description[0]) + weather.Description.Substring(1);
            WeatherLocationLabel.Text = weather.LocationName;
            
            // Update gradient based on weather condition
            SetWeatherGradient(weather.Condition);
        }

        private void UpdateWeatherLocationLabel()
        {
            // This method is no longer needed as location comes from WeatherData
            // Keeping for backwards compatibility but it won't be called
        }

        private void SetWeatherGradient(string condition)
        {
            Color startColor, endColor;
            bool isDarkMode = App.CurrentSettings.IsDarkMode;

            switch (condition.ToLower())
            {
                case "clear":
                    // Clear gradient (matches center hero section)
                    startColor = (Color)ColorConverter.ConvertFromString("#6366F1"); // blue
                    endColor = (Color)ColorConverter.ConvertFromString("#089DDA"); // blue
                    break;

                case "clouds":
                    // Cloudy gradient
                    if (isDarkMode)
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#6B7280"); // gray
                        endColor = (Color)ColorConverter.ConvertFromString("#475569"); // gray
                    }
                    else
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#D1D5DB"); // gray
                        endColor = (Color)ColorConverter.ConvertFromString("#94A3B8"); // gray
                    }
                    break;

                case "rain":
                case "drizzle":
                    // Rainy gradient
                    if (isDarkMode)
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#1E3A8A"); // deep blue
                        endColor = (Color)ColorConverter.ConvertFromString("#1E40AF"); // deep blue
                    }
                    else
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#1E40AF"); // blue
                        endColor = (Color)ColorConverter.ConvertFromString("#3B82F6"); // lighter blue
                    }
                    break;

                case "snow":
                    // Snowy gradient
                    if (isDarkMode)
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#60A5FA"); // blue; should be updated to better snowy colors
                        endColor = (Color)ColorConverter.ConvertFromString("#3B82F6"); // blue; should be updated to better snowy colors
                    }
                    else
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#DBEAFE"); // blue; should be updated to better snowy colors
                        endColor = (Color)ColorConverter.ConvertFromString("#93C5FD"); // blue; should be updated to better snowy colors
                    }
                    break;

                case "thunderstorm":
                    // Storm gradient
                    if (isDarkMode)
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#3730A3");
                        endColor = (Color)ColorConverter.ConvertFromString("#374151");
                    }
                    else
                    {
                        startColor = (Color)ColorConverter.ConvertFromString("#4C1D95");
                        endColor = (Color)ColorConverter.ConvertFromString("#6B7280");
                    }
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

        private void ScheduleCard_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to the Schedule tab
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Find the Schedule tab button and trigger its click
                var scheduleTab = mainWindow.FindName("ScheduleTab") as Button;
                if (scheduleTab != null)
                {
                    scheduleTab.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                }
            }
        }
    }
}
