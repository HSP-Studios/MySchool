using MySchool.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MySchool.Windows
{
    public partial class TimetableEditorDialog : Window
    {
      private TimetableData? _timetableData;
        private readonly Dictionary<string, EditableDaySchedule> _editableDays = new();
        private EditableDaySchedule? _currentDay;
        private EditablePeriod? _draggedPeriod;
 private int _draggedPeriodOriginalIndex;

        public bool DataChanged { get; private set; } = false;

        public TimetableEditorDialog()
        {
         InitializeComponent();
       LoadTimetableData();
  PopulateDayComboBox();
            SetupDragDrop();
 }

        /// <summary>
     /// Setup drag and drop for DataGrid rows
   /// </summary>
        private void SetupDragDrop()
   {
  PeriodsDataGrid.PreviewMouseLeftButtonDown += DataGrid_PreviewMouseLeftButtonDown;
PeriodsDataGrid.MouseMove += DataGrid_MouseMove;
            PeriodsDataGrid.Drop += DataGrid_Drop;
      PeriodsDataGrid.AllowDrop = true;
            PeriodsDataGrid.DragOver += DataGrid_DragOver;
        }

      /// <summary>
        /// Handle mouse down to initiate drag
        /// </summary>
      private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
   {
  // If this is a double-click, don't initiate drag - allow editing instead
     if (e.ClickCount == 2)
   {
    return;
  }

 // Reset dragged period at the start of any click
       _draggedPeriod = null;
   _draggedPeriodOriginalIndex = -1;

  if (e.OriginalSource is FrameworkElement element)
  {
        var row = FindVisualParent<DataGridRow>(element);
       if (row != null && row.Item is EditablePeriod period && _currentDay != null)
      {
 _draggedPeriod = period;
 _draggedPeriodOriginalIndex = _currentDay.Periods.IndexOf(period);
       }
   }
  }

        /// <summary>
     /// Handle mouse move to start drag operation
     /// </summary>
        private void DataGrid_MouseMove(object sender, MouseEventArgs e)
        {
    // Only start drag if left button is pressed, we have a dragged period, and current day is not null
            if (e.LeftButton == MouseButtonState.Pressed && 
    _draggedPeriod != null && 
  _currentDay != null &&
     _currentDay.Periods.Contains(_draggedPeriod))
  {
   try
    {
       _draggedPeriod.IsDragging = true;
   DragDrop.DoDragDrop(PeriodsDataGrid, _draggedPeriod, DragDropEffects.Move);
  }
          catch (Exception ex)
     {
 Logger.Warning("TimetableEditor", $"Drag operation failed: {ex.Message}");
   }
          finally
 {
         // Always reset dragging state and clear selection
     if (_draggedPeriod != null)
   {
          _draggedPeriod.IsDragging = false;
     }
            _draggedPeriod = null;
       
     // Clear selection after drag
     Dispatcher.BeginInvoke(new Action(() =>
   {
        PeriodsDataGrid.SelectedItem = null;
  PeriodsDataGrid.UnselectAll();
 }), System.Windows.Threading.DispatcherPriority.Background);
         }
     }
        }

 /// <summary>
        /// Handle drag over to show dynamic reordering
        /// </summary>
private void DataGrid_DragOver(object sender, DragEventArgs e)
        {
            // Safety checks
            if (_draggedPeriod == null || _currentDay == null || _currentDay.Periods == null)
    {
    e.Effects = DragDropEffects.None;
 e.Handled = true;
      return;
      }

   e.Effects = DragDropEffects.Move;

      // Find the row being dragged over
            var targetRow = FindVisualParent<DataGridRow>((DependencyObject)e.OriginalSource);
            if (targetRow != null && 
          targetRow.Item is EditablePeriod targetPeriod && 
   targetPeriod != _draggedPeriod &&
                _currentDay.Periods.Contains(_draggedPeriod) &&
          _currentDay.Periods.Contains(targetPeriod))
            {
            int draggedIndex = _currentDay.Periods.IndexOf(_draggedPeriod);
       int targetIndex = _currentDay.Periods.IndexOf(targetPeriod);

    if (draggedIndex != targetIndex && draggedIndex >= 0 && targetIndex >= 0)
         {
       try
    {
      // Move the dragged period
    _currentDay.Periods.Move(draggedIndex, targetIndex);

      // Dynamically renumber all periods based on their new positions
  for (int i = 0; i < _currentDay.Periods.Count; i++)
 {
          _currentDay.Periods[i].PeriodNumber = i + 1;
            }

      PeriodsDataGrid.Items.Refresh();
        }
       catch (Exception ex)
{
            Logger.Warning("TimetableEditor", $"Failed to reorder during drag: {ex.Message}");
   }
    }
            }

 e.Handled = true;
        }

        /// <summary>
     /// Handle drop to finalize the drag operation
        /// </summary>
        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
 if (_draggedPeriod != null && _currentDay != null)
  {
       try
       {
  DataChanged = true;
        Logger.Info("TimetableEditor", $"Period {_draggedPeriod.PeriodNumber} reordered in {_currentDay.Day}");

       // Reset IsDragging immediately
     _draggedPeriod.IsDragging = false;

    // Clear selection and refresh with a delay to ensure it happens after all drag events
       var tempPeriod = _draggedPeriod;
      Dispatcher.BeginInvoke(new Action(() =>
      {
     // Make sure IsDragging is false
   if (tempPeriod != null)
  {
   tempPeriod.IsDragging = false;
 }
            
       // Clear selection
        PeriodsDataGrid.SelectedItem = null;
        PeriodsDataGrid.UnselectAll();
 
    // Force a complete refresh
             PeriodsDataGrid.Items.Refresh();
    PeriodsDataGrid.UpdateLayout();
        }), System.Windows.Threading.DispatcherPriority.Background);
           }
      catch (Exception ex)
     {
     Logger.Warning("TimetableEditor", $"Failed to finalize drop: {ex.Message}");
           }
     finally
    {
     if (_draggedPeriod != null)
          {
   _draggedPeriod.IsDragging = false;
                }
    _draggedPeriod = null;
 }
            }
     else
   {
           if (_draggedPeriod != null)
         {
          _draggedPeriod.IsDragging = false;
          }
    _draggedPeriod = null;
      }

      e.Handled = true;
        }

        /// <summary>
        /// Find a visual parent of a specific type
     /// </summary>
        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
  {
            DependencyObject? parent = child;
    while (parent != null)
            {
   if (parent is T typedParent)
            {
        return typedParent;
       }
    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
 }
      return null;
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

              // Set up time change callbacks for auto-reordering
  foreach (var period in _currentDay.Periods)
    {
             period.SetTimeChangedCallback(() => {
         // Auto-reorder by time when any period's time changes
    _currentDay.AutoReorderByTime();
          PeriodsDataGrid.Items.Refresh();
         DataChanged = true;
      });
                }

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

            // Set up time change callback for auto-reordering
         newPeriod.SetTimeChangedCallback(() => {
             _currentDay.AutoReorderByTime();
   PeriodsDataGrid.Items.Refresh();
                DataChanged = true;
            });

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
     /// Open Find and Replace dialog to find and replace subject names across all days
        /// </summary>
        private void FindReplaceButton_Click(object sender, RoutedEventArgs e)
        {
try
       {
        Logger.Info("TimetableEditor", "User opened Find and Replace dialog");
          
        var dialog = new FindReplaceDialog
         {
   Owner = this
       };

if (dialog.ShowDialog() == true && dialog.ShouldReplace)
    {
              string findText = dialog.FindText;
       string replaceText = dialog.ReplaceText;
         int replacementCount = 0;

      // Search and replace across all days
         foreach (var daySchedule in _editableDays.Values)
      {
    foreach (var period in daySchedule.Periods)
{
     // Case-insensitive comparison for subject names
        if (period.Subject.Equals(findText, StringComparison.OrdinalIgnoreCase))
      {
        period.Subject = replaceText;
       replacementCount++;
   }
           }
       }

   // Refresh the current day's display
     if (_currentDay != null)
    {
      PeriodsDataGrid.Items.Refresh();
              }

      if (replacementCount > 0)
   {
            DataChanged = true;
           Logger.Info("TimetableEditor", $"Replaced '{findText}' with '{replaceText}' - {replacementCount} occurrence(s)");
      
          MessageBox.Show(
$"Successfully replaced {replacementCount} occurrence(s) of '{findText}' with '{replaceText}' across all days.",
         "Replace Complete",
  MessageBoxButton.OK,
          MessageBoxImage.Information);
      }
    else
    {
         Logger.Info("TimetableEditor", $"No occurrences of '{findText}' found");
  
      MessageBox.Show(
     $"No occurrences of '{findText}' were found.",
      "No Matches Found",
    MessageBoxButton.OK,
         MessageBoxImage.Information);
      }
      }
   }
      catch (Exception ex)
  {
       Logger.Error("TimetableEditor", "Failed to perform find and replace", ex);
       MessageBox.Show(
     $"Failed to perform find and replace: {ex.Message}",
  "Error",
  MessageBoxButton.OK,
        MessageBoxImage.Error);
            }
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

      // Check for overlapping periods
      var overlaps = FindOverlappingPeriods(daySchedule);
        errors.AddRange(overlaps);
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
        /// Find overlapping periods in a day schedule
      /// </summary>
  private List<string> FindOverlappingPeriods(EditableDaySchedule daySchedule)
        {
var overlaps = new List<string>();
 var periods = daySchedule.Periods
   .Where(p => TimeSpan.TryParse(p.StartTime, out _) && TimeSpan.TryParse(p.EndTime, out _))
       .OrderBy(p => p.GetStartTimeSpan())
        .ToList();

         for (int i = 0; i < periods.Count - 1; i++)
   {
          var currentPeriod = periods[i];
             var currentStart = TimeSpan.Parse(currentPeriod.StartTime);
     var currentEnd = TimeSpan.Parse(currentPeriod.EndTime);

    for (int j = i + 1; j < periods.Count; j++)
    {
       var nextPeriod = periods[j];
       var nextStart = TimeSpan.Parse(nextPeriod.StartTime);
                    var nextEnd = TimeSpan.Parse(nextPeriod.EndTime);

          // Check if periods overlap (not just touching)
          // Periods are overlapping if one starts before the other ends AND ends after the other starts
         if (currentStart < nextEnd && currentEnd > nextStart)
        {
      overlaps.Add($"{daySchedule.Day} - Overlapping periods: Period {currentPeriod.PeriodNumber} ({currentPeriod.Subject}, {currentPeriod.StartTime}-{currentPeriod.EndTime}) overlaps with Period {nextPeriod.PeriodNumber} ({nextPeriod.Subject}, {nextPeriod.StartTime}-{nextPeriod.EndTime})");
      }
   }
          }

        return overlaps;
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
