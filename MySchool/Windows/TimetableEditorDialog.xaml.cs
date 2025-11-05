using MySchool.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MySchool.Windows
{
    public partial class TimetableEditorDialog : Window
    {
      private TimetableData? _timetableData;
        private readonly Dictionary<string, EditableDaySchedule> _editableDays = new();
        private EditableDaySchedule? _currentDay;

        public bool DataChanged { get; private set; } = false;

        public TimetableEditorDialog()
        {
   InitializeComponent();
            LoadTimetableData();
   PopulateDayComboBox();
   }

  /// <summary>
 /// Load the current timetable data from file
        /// </summary>
  private void LoadTimetableData()
        {
try
            {
 _timetableData = TimetableManager.LoadLatestTimetable();

     if (_timetableData == null)
    {
        Logger.Warning("TimetableEditor", "No timetable data found");
         MessageBox.Show(
    "No timetable found. Please upload a timetable first.",
   "No Timetable",
    MessageBoxButton.OK,
MessageBoxImage.Information);
     DialogResult = false;
          Close();
              return;
            }

         // Convert to editable format
     foreach (var day in _timetableData.Timetable)
            {
         _editableDays[day.Day] = EditableDaySchedule.FromDaySchedule(day);
      }

    Logger.Info("TimetableEditor", $"Loaded timetable with {_editableDays.Count} days");
            }
         catch (Exception ex)
            {
    Logger.Error("TimetableEditor", "Failed to load timetable data", ex);
      MessageBox.Show(
         $"Failed to load timetable: {ex.Message}",
        "Error",
         MessageBoxButton.OK,
      MessageBoxImage.Error);
         DialogResult = false;
   Close();
}
        }

        /// <summary>
        /// Populate the day combo box with days from timetable
    /// </summary>
        private void PopulateDayComboBox()
     {
 DayComboBox.Items.Clear();

      // Standard week days in order
      var weekDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            foreach (var day in weekDays)
       {
             if (_editableDays.ContainsKey(day))
    {
               DayComboBox.Items.Add(day);
                }
      }

   // Select first day if available
          if (DayComboBox.Items.Count > 0)
            {
          DayComboBox.SelectedIndex = 0;
 }
        }

        /// <summary>
        /// Handle day selection change
        /// </summary>
        private void DayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
      if (DayComboBox.SelectedItem is string selectedDay)
            {
                LoadDaySchedule(selectedDay);
    }
        }

        /// <summary>
  /// Load the schedule for the selected day
        /// </summary>
        private void LoadDaySchedule(string day)
        {
      if (_editableDays.TryGetValue(day, out var daySchedule))
            {
      _currentDay = daySchedule;
       PeriodsDataGrid.ItemsSource = _currentDay.Periods;

                // Show/hide no periods message
        NoPeriodsPanel.Visibility = _currentDay.Periods.Count == 0 
            ? Visibility.Visible 
           : Visibility.Collapsed;

            Logger.Debug("TimetableEditor", $"Loaded {_currentDay.Periods.Count} periods for {day}");
          }
    }

 /// <summary>
  /// Add a new period to the current day
        /// </summary>
      private void AddPeriodButton_Click(object sender, RoutedEventArgs e)
        {
  if (_currentDay == null)
  {
     MessageBox.Show(
         "Please select a day first.",
         "No Day Selected",
           MessageBoxButton.OK,
         MessageBoxImage.Warning);
 return;
     }

  // Calculate next period number
            int nextPeriodNumber = _currentDay.Periods.Count > 0 
     ? _currentDay.Periods.Max(p => p.PeriodNumber) + 1 
            : 1;

        // Create new period with default values
            var newPeriod = new EditablePeriod
            {
         PeriodNumber = nextPeriodNumber,
       Subject = "New Period",
 Teacher = string.Empty,
       Room = string.Empty,
            StartTime = "09:00",
     EndTime = "10:00",
       IsBreak = false
  };

            _currentDay.Periods.Add(newPeriod);
  
     // Update visibility
            NoPeriodsPanel.Visibility = Visibility.Collapsed;

            Logger.Info("TimetableEditor", $"Added new period {nextPeriodNumber} to {_currentDay.Day}");
   DataChanged = true;
        }

        /// <summary>
 /// Delete a period from the current day
        /// </summary>
  private void DeletePeriodButton_Click(object sender, RoutedEventArgs e)
        {
    if (sender is Button button && button.Tag is EditablePeriod period)
 {
     var result = MessageBox.Show(
          $"Are you sure you want to delete Period {period.PeriodNumber} ({period.Subject})?",
         "Confirm Delete",
   MessageBoxButton.YesNo,
        MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes && _currentDay != null)
                {
     _currentDay.Periods.Remove(period);
  
          // Update visibility
 NoPeriodsPanel.Visibility = _currentDay.Periods.Count == 0 
      ? Visibility.Visible 
           : Visibility.Collapsed;

             Logger.Info("TimetableEditor", $"Deleted period {period.PeriodNumber} from {_currentDay.Day}");
           DataChanged = true;
          }
            }
        }

   /// <summary>
        /// Reorder periods by renumbering them sequentially
        /// </summary>
        private void ReorderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDay == null || _currentDay.Periods.Count == 0)
            {
           return;
            }

     // Sort by period number first, then renumber
            var sortedPeriods = _currentDay.Periods.OrderBy(p => p.PeriodNumber).ToList();
   
            for (int i = 0; i < sortedPeriods.Count; i++)
{
   sortedPeriods[i].PeriodNumber = i + 1;
     }

    // Refresh the DataGrid
            PeriodsDataGrid.Items.Refresh();

            Logger.Info("TimetableEditor", $"Reordered {sortedPeriods.Count} periods for {_currentDay.Day}");
          DataChanged = true;

            MessageBox.Show(
     $"Periods have been renumbered sequentially (1-{sortedPeriods.Count}).",
       "Reorder Complete",
  MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Save changes to timetable
      /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
      {
         try
        {
   if (_timetableData == null)
              {
         Logger.Error("TimetableEditor", "No timetable data to save");
                return;
    }

        // Validate all periods before saving
  if (!ValidateAllPeriods())
       {
   return;
       }

   // Convert editable data back to timetable format
              _timetableData.Timetable.Clear();
         foreach (var editableDay in _editableDays.Values)
                {
         _timetableData.Timetable.Add(editableDay.ToDaySchedule());
           }

         // Save to file
          var latestFile = TimetableManager.GetLatestTimetableFile();
        if (string.IsNullOrEmpty(latestFile))
  {
             Logger.Error("TimetableEditor", "No timetable file found to save to");
          MessageBox.Show(
            "No timetable file found to save changes.",
          "Error",
      MessageBoxButton.OK,
          MessageBoxImage.Error);
        return;
     }

        TimetableManager.SaveTimetable(_timetableData, latestFile);

      Logger.Info("TimetableEditor", "Timetable saved successfully");
         MessageBox.Show(
         "Timetable changes saved successfully!",
     "Save Complete",
     MessageBoxButton.OK,
        MessageBoxImage.Information);

         DialogResult = true;
      Close();
            }
 catch (Exception ex)
            {
Logger.Error("TimetableEditor", "Failed to save timetable", ex);
         MessageBox.Show(
  $"Failed to save timetable: {ex.Message}",
       "Error",
         MessageBoxButton.OK,
    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Validate all periods across all days
        /// </summary>
        private bool ValidateAllPeriods()
   {
            var errors = new List<string>();

      foreach (var daySchedule in _editableDays.Values)
       {
          foreach (var period in daySchedule.Periods)
     {
     // Validate subject
        if (string.IsNullOrWhiteSpace(period.Subject))
               {
          errors.Add($"{daySchedule.Day} - Period {period.PeriodNumber}: Subject cannot be empty");
                }

         // Validate times
     if (!TimeSpan.TryParse(period.StartTime, out _))
        {
     errors.Add($"{daySchedule.Day} - Period {period.PeriodNumber}: Invalid start time format (use HH:MM)");
 }

         if (!TimeSpan.TryParse(period.EndTime, out _))
 {
    errors.Add($"{daySchedule.Day} - Period {period.PeriodNumber}: Invalid end time format (use HH:MM)");
       }

              // Validate start time is before end time
    if (TimeSpan.TryParse(period.StartTime, out var start) && 
TimeSpan.TryParse(period.EndTime, out var end))
     {
     if (start >= end)
         {
      errors.Add($"{daySchedule.Day} - Period {period.PeriodNumber}: Start time must be before end time");
       }
            }
      }
      }

            if (errors.Any())
 {
            MessageBox.Show(
       "Please fix the following errors:\n\n" + string.Join("\n", errors.Take(10)) +
      (errors.Count > 10 ? $"\n\n...and {errors.Count - 10} more" : ""),
            "Validation Errors",
 MessageBoxButton.OK,
       MessageBoxImage.Warning);
   return false;
            }

return true;
        }

        /// <summary>
  /// Cancel editing and close dialog
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
        if (DataChanged)
            {
 var result = MessageBox.Show(
          "You have unsaved changes. Are you sure you want to cancel?",
         "Unsaved Changes",
    MessageBoxButton.YesNo,
    MessageBoxImage.Question);

             if (result == MessageBoxResult.No)
                {
      return;
     }
   }

   DialogResult = false;
            Close();
    }

        /// <summary>
        /// Close dialog
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
          CancelButton_Click(sender, e);
        }
    }
}
