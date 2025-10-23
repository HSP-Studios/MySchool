using System.Reflection;

namespace MySchool.Classes
{
    /// <summary>
    /// Helper class to access build information generated during compilation
    /// </summary>
    internal static class BuildInfoHelper
    {
        private static string? _version;
        private static string? _buildNumber;
        private static string? _buildDate;

        /// <summary>
        /// Gets the application version from the generated BuildInfo class
        /// </summary>
        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    try
                    {
                        var buildInfoType = Type.GetType("MySchool.Generated.BuildInfo, MySchool");
                        if (buildInfoType != null)
                        {
                            var versionField = buildInfoType.GetField("Version", BindingFlags.Public | BindingFlags.Static);
                            _version = versionField?.GetValue(null) as string ?? "Unknown";
                        }
                        else
                        {
                            _version = "Unknown";
                        }
                    }
                    catch
                    {
                        _version = "Unknown";
                    }
                }
                return _version;
            }
        }

        /// <summary>
        /// Gets the build number in format YYMMDDRRRR
        /// </summary>
        public static string BuildNumber
        {
            get
            {
                if (_buildNumber == null)
                {
                    try
                    {
                        var buildInfoType = Type.GetType("MySchool.Generated.BuildInfo, MySchool");
                        if (buildInfoType != null)
                        {
                            var buildNumberField = buildInfoType.GetField("BuildNumber", BindingFlags.Public | BindingFlags.Static);
                            _buildNumber = buildNumberField?.GetValue(null) as string ?? "Unknown";
                        }
                        else
                        {
                            _buildNumber = "Unknown";
                        }
                    }
                    catch
                    {
                        _buildNumber = "Unknown";
                    }
                }
                return _buildNumber;
            }
        }

        /// <summary>
        /// Gets the build date in format YY-MM-DD
        /// </summary>
        public static string BuildDate
        {
            get
            {
                if (_buildDate == null)
                {
                    try
                    {
                        var buildInfoType = Type.GetType("MySchool.Generated.BuildInfo, MySchool");
                        if (buildInfoType != null)
                        {
                            var buildDateField = buildInfoType.GetField("BuildDate", BindingFlags.Public | BindingFlags.Static);
                            _buildDate = buildDateField?.GetValue(null) as string ?? "Unknown";
                        }
                        else
                        {
                            _buildDate = "Unknown";
                        }
                    }
                    catch
                    {
                        _buildDate = "Unknown";
                    }
                }
                return _buildDate;
            }
        }
    }
}
