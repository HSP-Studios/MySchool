using System;
using System.Collections.Generic;
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
                return new List<string>();
            }
        }

        public static string? GetLatestTimetableFile()
        {
            var files = GetAllTimetableFiles();
            return files.FirstOrDefault();
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
    }
}

