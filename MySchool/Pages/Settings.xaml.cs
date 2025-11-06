using MySchool.Classes;
using MySchool.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MySchool.Pages
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Page
	{
		private ScrollViewer? _settingsScrollViewer;

		public Settings()
		{
			InitializeComponent();
			Loaded += Settings_Loaded;
			PreviewMouseWheel += Settings_PreviewMouseWheel;
		}

		private void Settings_Loaded(object sender, RoutedEventArgs e)
		{
			// Initialize toggle from current settings
			try
			{
				// Set the appropriate theme button as checked based on saved theme
				string currentTheme = App.CurrentSettings.ThemeName ?? "Light";
				switch (currentTheme.ToLower())
				{
					case "light":
						LightThemeButton.IsChecked = true;
						break;
					case "dark":
						DarkThemeButton.IsChecked = true;
						break;
					case "midnight":
						MidnightThemeButton.IsChecked = true;
						break;
					case "nord":
						NordThemeButton.IsChecked = true;
						break;
					case "ocean":
						OceanThemeButton.IsChecked = true;
						break;
					case "forest":
						ForestThemeButton.IsChecked = true;
						break;
					default:
						LightThemeButton.IsChecked = true;
						break;
				}

				// Force template application before setting accent colors
				LightThemeButton.ApplyTemplate();
				DarkThemeButton.ApplyTemplate();
				MidnightThemeButton.ApplyTemplate();
				NordThemeButton.ApplyTemplate();
				OceanThemeButton.ApplyTemplate();
				ForestThemeButton.ApplyTemplate();

				// Set accent colors for theme buttons
				SetAccentColor(LightThemeButton, "#4F46E5");  // Primary: Indigo
				SetAccentColor(DarkThemeButton, "#818CF8");   // Primary: Light Indigo
				SetAccentColor(MidnightThemeButton, "#8B5CF6"); // Primary: Purple
				SetAccentColor(NordThemeButton, "#88C0D0");   // Primary: Light Blue
				SetAccentColor(OceanThemeButton, "#14B8A6");  // Primary: Teal
				SetAccentColor(ForestThemeButton, "#10B981"); // Primary: Green

				UserNameTextBox.Text = App.CurrentSettings.UserName;
				UpdateLocationDisplay();
				UpdateBuildInfo();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failed to initialize settings: " + ex);
				LightThemeButton.IsChecked = true; // fallback to default
			}

			// Find the ScrollViewer instance
			_settingsScrollViewer = FindScrollViewer(this);
		}

		private void Settings_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (_settingsScrollViewer != null)
			{
				// Reduce scroll speed by dividing delta
				int scrollDelta = e.Delta / 6; // Lower divisor = slower scroll
				_settingsScrollViewer.ScrollToVerticalOffset(_settingsScrollViewer.VerticalOffset - scrollDelta);
				e.Handled = true;
			}
		}

		private ScrollViewer? FindScrollViewer(DependencyObject parent)
		{
			if (parent is ScrollViewer sv)
				return sv;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				var result = FindScrollViewer(child);
				if (result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Sets the accent color indicator for a theme button
		/// </summary>
		private void SetAccentColor(RadioButton button, string hexColor)
		{
			try
			{
				// Ensure the button's template is applied
				button.ApplyTemplate();

				// Force a layout update to ensure visual tree is ready
				button.UpdateLayout();

				// Get the template
				if (button.Template.FindName("AccentIndicator", button) is Border accentIndicator)
				{
					accentIndicator.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
					System.Diagnostics.Debug.WriteLine($"Set accent color for {button.Name}: {hexColor}");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine($"AccentIndicator not found for {button.Name}");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to set accent color for {button.Name}: {ex.Message}");
			}
		}

		private void UpdateLocationDisplay()
		{
			if (App.CurrentSettings.WeatherLocation.HasValue)
			{
				var loc = App.CurrentSettings.WeatherLocation.Value;
				var name = string.IsNullOrWhiteSpace(App.CurrentSettings.WeatherLocationName)
					? "Manual location"
					: App.CurrentSettings.WeatherLocationName;
				CurrentLocationText.Text = $"{name} ({loc.latitude:F2}, {loc.longitude:F2})";
			}
			else
			{
				CurrentLocationText.Text = "Using device location";
			}
		}

		private void UpdateBuildInfo()
		{
			try
			{
				VersionText.Text = BuildInfoHelper.Version;
				BuildNumberText.Text = BuildInfoHelper.BuildNumber;
				BuildDateText.Text = BuildInfoHelper.BuildDate;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failed to load build info: " + ex);
				VersionText.Text = "Unknown";
				BuildNumberText.Text = "Unknown";
				BuildDateText.Text = "Unknown";
			}
		}

		private void ThemeButton_Checked(object sender, RoutedEventArgs e)
		{
			if (sender is not RadioButton button) return;

			string themeName = button.Name switch
			{
				"LightThemeButton" => "Light",
				"DarkThemeButton" => "Dark",
				"MidnightThemeButton" => "Midnight",
				"NordThemeButton" => "Nord",
				"OceanThemeButton" => "Ocean",
				"ForestThemeButton" => "Forest",
				_ => "Light"
			};

			SetTheme(themeName);
		}

		private void SetTheme(string themeName)
		{
			App.CurrentSettings.ThemeName = themeName;
			SettingsService.Save(App.CurrentSettings);
			// Apply theme with cross-fade transition (0.5s)
			ThemeManager.ApplyThemeWithTransition(themeName, 0.5);
		}

		private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				App.CurrentSettings.UserName = UserNameTextBox.Text.Trim();
				SettingsService.Save(App.CurrentSettings);

				// Show warning if name exceeds 8 characters
				if (!string.IsNullOrWhiteSpace(UserNameTextBox.Text) && UserNameTextBox.Text.Trim().Length > 8)
				{
					NameLengthWarning.Visibility = Visibility.Visible;
				}
				else
				{
					NameLengthWarning.Visibility = Visibility.Collapsed;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failed to save user name: " + ex);
			}
		}

		private void UploadTimetableButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var dialog = new TimetableUploadDialog
				{
					Owner = Window.GetWindow(this)
				};
				dialog.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to open upload dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void EditTimetableButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Logger.Info("Settings", "User opened timetable editor");
				var dialog = new TimetableEditorDialog
				{
					Owner = Window.GetWindow(this)
				};
				
				var result = dialog.ShowDialog();
				
				if (result == true && dialog.DataChanged)
				{
					Logger.Info("Settings", "Timetable editor changes saved successfully");
					MessageBox.Show(
						"Your timetable has been updated!\n\nChanges will be reflected immediately in the Home and Schedule tabs.",
						"Timetable Updated",
						MessageBoxButton.OK,
						MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Settings", "Failed to open timetable editor", ex);
				MessageBox.Show($"Failed to open timetable editor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void RequestLocationButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var location = await WeatherService.GetLocationAsync();

				if (location.HasValue)
				{
					// Clear manual location to use device location
					App.CurrentSettings.WeatherLocation = null;
					App.CurrentSettings.WeatherLocationName = string.Empty;
					SettingsService.Save(App.CurrentSettings);
					UpdateLocationDisplay();

					MessageBox.Show($"Location permission granted!\nYour coordinates: {location.Value.latitude:F4}, {location.Value.longitude:F4}",
						"Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show("Unable to access location. Please check your location permissions in Windows Settings.",
						"Location Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to request location: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ManualLocationButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var dialog = new ManualLocationDialog
				{
					Owner = Window.GetWindow(this)
				};

				if (dialog.ShowDialog() == true)
				{
					UpdateLocationDisplay();
					MessageBox.Show("Location saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to open location dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void ReprocessTimetableButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Settings", "User initiated timetable re-processing");

			// Create progress dialog
			var (progressDialog, stackPanel, progressBar, statusText) = CreateProgressDialog("Re-processing Timetable", "Processing timetable...");

			// Disable the button while processing
			ReprocessTimetableButton.IsEnabled = false;
			Logger.Debug("Settings", "Re-process button disabled during operation");

			// Show the dialog non-modally
			progressDialog.Show();

			try
			{
				// Run processing on a background task
				var changes = await Task.Run(() => TimetableManager.ReprocessLatestTimetable());

				// Close progress dialog
				progressDialog.Close();

				// Show results
				string resultMessage;
				if (changes.Any())
				{
					resultMessage = $"Timetable re-processed successfully!\n\n" +
				 $"Grouped {changes.Count} consecutive period(s):\n\n"
				 + string.Join("\n", changes.Take(5));

					if (changes.Count > 5)
					{
						resultMessage += $"\n... and {changes.Count - 5} more";
					}

					Logger.Info("Settings", $"Timetable re-processed successfully with {changes.Count} change(s)");
				}
				else
				{
					resultMessage = "Timetable re-processed successfully!\n\n" +
 "No consecutive periods were found to group.\n" +
	 "Your timetable is already optimized.";
					Logger.Info("Settings", "Timetable re-processed - no changes needed");
				}

				MessageBox.Show(resultMessage, "Re-processing Complete", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (FileNotFoundException)
			{
				progressDialog.Close();
				Logger.Warning("Settings", "No timetable file found for re-processing");
				MessageBox.Show(
			"No timetable found to re-process.\n\n" +
		   "Please upload a timetable first using the 'Upload Timetable' button.",
				 "No Timetable Found",
			  MessageBoxButton.OK,
				   MessageBoxImage.Warning);
			}
			catch (Exception ex)
			{
				progressDialog.Close();
				Logger.Error("Settings", "Failed to re-process timetable", ex);

				string logPath = Logger.GetLogFilePath();
				string errorMessage = $"Failed to re-process timetable:\n\n{ex.Message}\n\n" +
					 $"Log file location:\n{logPath}";

				var errorDialog = new ErrorDialog("Re-processing Failed", errorMessage)
				{
					Owner = Window.GetWindow(this)
				};
				errorDialog.ShowDialog();

				if (errorDialog.OpenLogsRequested)
				{
					OpenLogFolder();
				}
			}
			finally
			{
				ReprocessTimetableButton.IsEnabled = true;
				Logger.Debug("Settings", "Re-process button re-enabled");
			}
		}

		private void OpenLogFolder()
		{
			try
			{
				string logPath = Logger.GetLogFilePath();
				string logFolder = System.IO.Path.GetDirectoryName(logPath) ?? string.Empty;

				if (Directory.Exists(logFolder))
				{
					Process.Start("explorer.exe", logFolder);
					Logger.Info("Settings", $"Opened log folder: {logFolder}");
				}
				else
				{
					Logger.Warning("Settings", $"Log folder does not exist: {logFolder}");
					MessageBox.Show($"Log folder not found:\n{logFolder}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Settings", "Failed to open log folder", ex);
				MessageBox.Show($"Failed to open log folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void CheckUpdatesButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Settings", "User initiated manual update check");

			var (checkingDialog, stackPanel, progressBar, statusText) = CreateProgressDialog("Checking for Updates", "Checking for updates...");

			// Track dialog state
			bool dialogClosed = false;
			checkingDialog.Closed += (s, args) => dialogClosed = true;

			// Disable the button while checking
			CheckUpdatesButton.IsEnabled = false;
			Logger.Debug("Settings", "Update check button disabled during operation");

			// Show the dialog non-modally
			checkingDialog.Show();

			// Helper method to safely close the dialog
			void SafeCloseDialog()
			{
				try
				{
					if (!dialogClosed && checkingDialog.IsLoaded)
					{
						checkingDialog.Close();
						dialogClosed = true;
						Logger.Debug("Settings", "Progress dialog closed successfully");
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("Settings", "Error closing progress dialog", ex);
					// Ignore errors when closing dialog
				}
			}

			try
			{
				Logger.Info("Settings", "Starting update check process");
				var updateInfo = await App.CheckForUpdatesAsync();

				SafeCloseDialog();

				if (updateInfo != null)
				{
					Logger.Info("Settings", $"Update available: Version {updateInfo.TargetFullRelease.Version}");

					var result = MessageBox.Show(
					 $"A new version of MySchool is available!\n\n" +
					 $"New Version: {updateInfo.TargetFullRelease.Version}\n\n" +
					 $"Would you like to download and install the update now?\n" +
			   $"The application will restart after the update.",
				   "Update Available",
					   MessageBoxButton.YesNo,
						MessageBoxImage.Information);

					if (result == MessageBoxResult.Yes)
					{
						Logger.Info("Settings", "User accepted update installation");

						// Create a NEW dialog for the download phase (don't reuse the closed one)
						var (downloadDialog, downloadPanel, downloadProgressBar, downloadStatusText) = 
							CreateProgressDialog("Downloading Update", "Downloading update...");
						
						bool downloadDialogClosed = false;
						downloadDialog.Closed += (s, args) => downloadDialogClosed = true;
						
						downloadDialog.Show();
						Logger.Debug("Settings", "Download progress dialog opened");

						Logger.Info("Settings", "Starting update download and installation");
						bool success = await App.DownloadAndApplyUpdateAsync(updateInfo);

						// Close download dialog BEFORE restart
						try
						{
							if (!downloadDialogClosed && downloadDialog.IsLoaded)
							{
								downloadDialog.Close();
								downloadDialogClosed = true;
								Logger.Debug("Settings", "Download dialog closed successfully");
							}
						}
						catch (Exception ex)
						{
							Logger.Warning("Settings", "Error closing download dialog", ex);
						}

						if (!success)
						{
							Logger.Error("Settings", "Update installation failed");

							string logPath = Logger.GetLogFilePath();

							string failMessage = "Failed to download or apply the update.\n\n" +
							"Possible causes:\n" +
							  "• Network connection interrupted\n" +
						   "• Insufficient disk space\n" +
							"• Permission denied\n\n" +
							$"Check the log file for details:\n{logPath}\n\n" +
								  "You can also download the update manually from:\n" +
			"https://github.com/HSP-Studios/MySchool/releases";

							var errorDialog = new ErrorDialog("Update Failed", failMessage)
							{
								Owner = Window.GetWindow(this)
							};
							errorDialog.ShowDialog();

							if (errorDialog.OpenLogsRequested)
							{
								OpenLogFolder();
							}
						}
						else
						{
							Logger.Info("Settings", "Update completed successfully - application should restart");
						}
					}
					else
					{
						Logger.Info("Settings", "User declined update installation");
					}
				}
				else
				{
					Logger.Info("Settings", "No updates available - user is on latest version");
					MessageBox.Show(
				"You are running the latest version of MySchool!",
				   "No Updates Available",
					  MessageBoxButton.OK,
				   MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				SafeCloseDialog();

				Logger.Error("Settings", "Update check failed with exception", ex);

				// Provide detailed error message based on exception type
				string errorTitle = "Update Check Failed";
				string errorMessage = ex.Message;

				// Get log file path for user reference
				string logPath = Logger.GetLogFilePath();

				// Add helpful suggestions based on the error
				if (errorMessage.Contains("internet connection") ||
				  errorMessage.Contains("timed out") ||
				   errorMessage.Contains("connect") ||
					  ex is System.Net.Http.HttpRequestException ||
				  ex is System.Net.WebException ||
					  ex is TaskCanceledException)
				{
					Logger.Warning("Settings", "Update check failed due to network issues");

					errorMessage += "\n\nTroubleshooting steps:\n" +
					 "• Verify you are connected to the internet\n" +
				  "• Check if your firewall is blocking the connection\n" +
						 "• Ensure GitHub.com is accessible from your network\n" +
					  "• Try again in a few moments\n\n" +
			  "You can also check for updates manually at:\n" +
			"https://github.com/HSP-Studios/MySchool/releases";
				}
				else if (ex is UnauthorizedAccessException || errorMessage.Contains("permission"))
				{
					Logger.Warning("Settings", "Update check failed due to permission issues");
					errorTitle = "Permission Denied";
					errorMessage += "\n\nThe application may need elevated permissions.\n" +
					   "Try running the application as Administrator.";
				}
				else
				{
					Logger.Warning("Settings", $"Update check failed with unexpected error: {ex.GetType().Name}");
					errorMessage += $"\n\nError Type: {ex.GetType().Name}\n\n" +
	 "Please try again later. If the problem persists,\n" +
		  "you can download updates manually from:\n" +
  "https://github.com/HSP-Studios/MySchool/releases";
				}

				errorMessage += $"\n\nLog file location:\n{logPath}";

				var errorDialog = new ErrorDialog(errorTitle, errorMessage)
				{
					Owner = Window.GetWindow(this)
				};
				errorDialog.ShowDialog();

				if (errorDialog.OpenLogsRequested)
				{
					OpenLogFolder();
				}
			}
			finally
			{
				CheckUpdatesButton.IsEnabled = true;
				Logger.Debug("Settings", "Update check button re-enabled");
			}
		}

		/// <summary>
		/// Creates a progress dialog window with the specified title and message
		/// </summary>
		/// <param name="title">Dialog window title</param>
		/// <param name="message">Status message to display</param>
		/// <returns>Tuple containing the Window, StackPanel, ProgressBar, and TextBlock for updates</returns>
		private (Window dialog, StackPanel panel, ProgressBar progressBar, TextBlock statusText) CreateProgressDialog(string title, string message)
		{
			var dialog = new Window
			{
				Title = title,
				Width = 400,
				Height = 150,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Owner = Window.GetWindow(this),
				ResizeMode = ResizeMode.NoResize,
				WindowStyle = WindowStyle.ToolWindow,
				Background = (Brush)Application.Current.Resources["Brush.Background"]
			};

			var stackPanel = new StackPanel
			{
				Margin = new Thickness(20),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			var progressBar = new ProgressBar
			{
				IsIndeterminate = true,
				Width = 300,
				Height = 20,
				Margin = new Thickness(0, 0, 0, 10)
			};

			var statusText = new TextBlock
			{
				Text = message,
				HorizontalAlignment = HorizontalAlignment.Center,
				Style = (Style)Application.Current.Resources["Text.Body"]
			};

			stackPanel.Children.Add(progressBar);
			stackPanel.Children.Add(statusText);
			dialog.Content = stackPanel;

			return (dialog, stackPanel, progressBar, statusText);
		}
	}
}
