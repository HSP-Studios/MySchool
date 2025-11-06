using System.IO;
using System.Text.Json;

namespace MySchool.Classes
{
    internal class TimetableManager
    {
        private static string GetTimetablesDirectory()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string timetablesPath = Path.Combine(appData, "MySchool", "timetables");

            if (!Directory.Exists(timetablesPath))
            {
                Directory.CreateDirectory(timetablesPath);
            }

            return timetablesPath;
        }

        public static List<string> GetAllTimetableFiles()
        {
            try
            {
                string timetablesDir = GetTimetablesDirectory();
                return Directory.GetFiles(timetablesDir, "*.json")
                    .OrderByDescending(f => f)
                    .ToList();
            }
            catch
            {
                // Intentionally catch all exceptions to ensure the application continues
                // gracefully if there are any issues accessing the timetables directory or files.
                return new List<string>();
            }
        }

        public static string? GetLatestTimetableFile()
        {
            var files = GetAllTimetableFiles();
            return files.FirstOrDefault();
        }

        public static string? GetLatestTimetablePdf()
        {
            try
            {
                string timetablesDir = GetTimetablesDirectory();
                var pdfFiles = Directory.GetFiles(timetablesDir, "*.pdf")
                    .OrderByDescending(f => f)
                    .ToList();
                return pdfFiles.FirstOrDefault();
            }
            catch
            {
                // Intentionally catch all exceptions and return null if unable to retrieve the latest timetable PDF.
                return null;
            }
        }

        public static T? LoadTimetable<T>(string filePath) where T : class
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return null;
            }
        }

        public static TimetableData? LoadLatestTimetable()
        {
            var latestFile = GetLatestTimetableFile();
            if (string.IsNullOrEmpty(latestFile))
                return null;

            return LoadTimetable<TimetableData>(latestFile);
        }

        public static (Period? current, Period? next) GetCurrentAndNextClass()
        {
            try
            {
                var timetable = LoadLatestTimetable();
                if (timetable == null)
                    return (null, null);

                var today = DateTime.Now.DayOfWeek.ToString();
                var currentTime = DateTime.Now.TimeOfDay;

                var todaySchedule = timetable.Timetable.FirstOrDefault(d =>
                    d.Day.Equals(today, StringComparison.OrdinalIgnoreCase));

                if (todaySchedule == null)
                    return (null, null);

                var sortedPeriods = todaySchedule.Periods.OrderBy(p => p.PeriodNumber).ToList();

                Period? currentPeriod = null;
                Period? nextPeriod = null;

                for (int i = 0; i < sortedPeriods.Count; i++)
                {
                    var period = sortedPeriods[i];

                    if (!TimeSpan.TryParse(period.StartTime, out var startTime) ||
                        !TimeSpan.TryParse(period.EndTime, out var endTime))
                        continue;

                    // Check if current time is within this period
                    if (currentTime >= startTime && currentTime < endTime)
                    {
                        currentPeriod = period;
                        // Next period is the one after current
                        if (i + 1 < sortedPeriods.Count)
                            nextPeriod = sortedPeriods[i + 1];
                        break;
                    }
                    // If current time is before this period's start, this is the next period
                    else if (currentTime < startTime && nextPeriod == null)
                    {
                        nextPeriod = period;
                    }
                }

                // If we have a current period but no next, check if there's a period after current
                if (currentPeriod != null && nextPeriod == null)
                {
                    var currentIndex = sortedPeriods.IndexOf(currentPeriod);
                    if (currentIndex + 1 < sortedPeriods.Count)
                        nextPeriod = sortedPeriods[currentIndex + 1];
                }

                return (currentPeriod, nextPeriod);
            }
            catch
            {
                return (null, null);
            }
        }

        // Returns true if the current time is after the last period's end time for today.
        public static bool IsAfterLastPeriodForToday()
        {
            try
            {
                var timetable = LoadLatestTimetable();
                if (timetable == null)
                    return false;

                var todayName = DateTime.Now.DayOfWeek.ToString();
                var todaySchedule = timetable.Timetable.FirstOrDefault(d =>
                    d.Day.Equals(todayName, StringComparison.OrdinalIgnoreCase));

                if (todaySchedule == null || todaySchedule.Periods == null || todaySchedule.Periods.Count == 0)
                    return false;

                // Find the maximum valid end time across all periods (classes and breaks)
                TimeSpan? lastEnd = null;
                foreach (var p in todaySchedule.Periods)
                {
                    if (TimeSpan.TryParse(p.EndTime, out var end))
                    {
                        if (lastEnd == null || end > lastEnd)
                        {
                            lastEnd = end;
                        }
                    }
                }

                if (lastEnd == null)
                    return false;

                return DateTime.Now.TimeOfDay >= lastEnd.Value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Groups consecutive periods with the same subject name into a single period.
        /// Returns a list of descriptions of what was changed.
        /// </summary>
        public static (TimetableData processedData, List<string> changes) GroupConsecutivePeriods(TimetableData timetableData)
        {
            Logger.Info("TimetableManager", "Starting to group consecutive periods");
            var changes = new List<string>();

            try
            {
                foreach (var day in timetableData.Timetable)
                {
                    Logger.Debug("TimetableManager", $"Processing day: {day.Day}");
                    var newPeriods = new List<Period>();
                    Period? currentGroup = null;

                    for (int i = 0; i < day.Periods.Count; i++)
                    {
                        var period = day.Periods[i];

                        // If this is the first period or subject is different from current group
                        if (currentGroup == null ||
                            !period.Subject.Equals(currentGroup.Subject, StringComparison.OrdinalIgnoreCase) ||
                            period.IsBreak != currentGroup.IsBreak)
                        {
                            // Save the previous group if it exists
                            if (currentGroup != null)
                            {
                                newPeriods.Add(currentGroup);
                            }

                            // Start a new group
                            currentGroup = new Period
                            {
                                PeriodNumber = period.PeriodNumber,
                                Subject = period.Subject,
                                Teacher = period.Teacher,
                                Room = period.Room,
                                StartTime = period.StartTime,
                                EndTime = period.EndTime,
                                IsBreak = period.IsBreak
                            };
                        }
                        else
                        {
                            // Extend the current group's end time
                            string oldEndTime = currentGroup.EndTime;
                            currentGroup.EndTime = period.EndTime;

                            // Log the grouping
                            string changeMessage = $"{day.Day}: Grouped consecutive '{period.Subject}' periods " +
                                    $"(Period {currentGroup.PeriodNumber} extended from {oldEndTime} to {period.EndTime})";
                            changes.Add(changeMessage);
                            Logger.Info("TimetableManager", changeMessage);
                        }
                    }

                    // Add the last group
                    if (currentGroup != null)
                    {
                        newPeriods.Add(currentGroup);
                    }

                    // Replace the day's periods with the grouped periods
                    day.Periods = newPeriods;
                }

                if (changes.Count == 0)
                {
                    Logger.Info("TimetableManager", "No consecutive periods found to group");
                }
                else
                {
                    Logger.Info("TimetableManager", $"Grouped {changes.Count} consecutive period(s)");
                }

                return (timetableData, changes);
            }
            catch (Exception ex)
            {
                Logger.Error("TimetableManager", "Failed to group consecutive periods", ex);
                throw;
            }
        }

        /// <summary>
        /// Processes a timetable by grouping consecutive periods.
        /// Saves the processed timetable to a new file with the same timestamp.
        /// Returns the list of changes made.
        /// </summary>
        public static List<string> ProcessAndSaveTimetable(string sourceFilePath)
        {
            Logger.Info("TimetableManager", $"Processing timetable file: {sourceFilePath}");

            try
            {
                // Load the timetable
                var timetableData = LoadTimetable<TimetableData>(sourceFilePath);
                if (timetableData == null)
                {
                    Logger.Error("TimetableManager", "Failed to load timetable data from file");
                    throw new InvalidOperationException("Failed to load timetable data");
                }

                // Process the timetable
                var (processedData, changes) = GroupConsecutivePeriods(timetableData);

                // Save back to the same file
                string formattedJson = JsonSerializer.Serialize(processedData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(sourceFilePath, formattedJson);

                Logger.Info("TimetableManager", $"Saved processed timetable to: {sourceFilePath}");

                return changes;
            }
            catch (Exception ex)
            {
                Logger.Error("TimetableManager", "Failed to process and save timetable", ex);
                throw;
            }
        }

        /// <summary>
        /// Re-processes the latest timetable file by grouping consecutive periods.
        /// Returns the list of changes made.
        /// </summary>
        public static List<string> ReprocessLatestTimetable()
        {
            Logger.Info("TimetableManager", "Starting to re-process latest timetable");

            var latestFile = GetLatestTimetableFile();
            if (string.IsNullOrEmpty(latestFile))
            {
                Logger.Warning("TimetableManager", "No timetable file found to re-process");
                throw new FileNotFoundException("No timetable file found");
            }

            return ProcessAndSaveTimetable(latestFile);
        }

        /// <summary>
        /// Save timetable data to a specified file
        /// </summary>
        public static void SaveTimetable(TimetableData timetableData, string filePath)
        {
            Logger.Info("TimetableManager", $"Saving timetable to: {filePath}");

            try
            {
                string formattedJson = JsonSerializer.Serialize(timetableData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, formattedJson);

                Logger.Info("TimetableManager", "Timetable saved successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("TimetableManager", "Failed to save timetable", ex);
                throw;
            }
        }
    }
}

