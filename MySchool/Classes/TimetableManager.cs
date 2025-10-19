using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    }
}

