using System.IO;
using System.Windows;
using System.Windows.Media;

namespace MySchool.Classes
{
    /// <summary>
    /// Helper class for managing font families in the application.
    /// Discovers and loads fonts from the resources/fonts directory.
 /// </summary>
    public static class FontManager
    {
    private static readonly Dictionary<string, string> _fontFamilyMap = new()
        {
       { "SF Pro", "SF-Pro.ttf#SF Pro" },
       { "Bitcount Grid Single", "Bitcount_Grid_Single/BitcountGridSingle.ttf#Bitcount Grid Single" },
    { "Creepster", "Creepster/Creepster-Regular.ttf#Creepster" },
    { "Inter", "Inter/static/Inter_18pt-Regular.ttf#Inter" },
    { "League Spartan", "League_Spartan/static/LeagueSpartan-Regular.ttf#League Spartan" },
     { "Montserrat", "Montserrat/static/Montserrat-Regular.ttf#Montserrat" },
     { "Nunito", "Nunito/static/Nunito-Regular.ttf#Nunito" },
         { "Playwrite AU TAS", "Playwrite_AU_TAS/PlaywriteAUTAS-Regular.ttf#Playwrite AU TAS" },
 { "Press Start 2P", "Press_Start_2P/PressStart2P-Regular.ttf#Press Start 2P" },
      { "Public Sans", "Public_Sans/static/PublicSans-Regular.ttf#Public Sans" },
     { "Roboto", "Roboto/static/Roboto-Regular.ttf#Roboto" },
        { "Sanchez", "Sanchez/Sanchez-Regular.ttf#Sanchez" },
     { "Sofia Sans", "Sofia_Sans/static/SofiaSans-Regular.ttf#Sofia Sans" },
        { "Ubuntu", "Ubuntu/Ubuntu-Regular.ttf#Ubuntu" },
            { "Zalando Sans Expanded", "Zalando_Sans_Expanded/static/ZalandoSansExpanded-Regular.ttf#Zalando Sans Expanded" }
      };

        /// <summary>
  /// Gets a list of all available font family names.
        /// </summary>
        public static List<string> GetAvailableFonts()
        {
      try
       {
        Logger.Debug("FontManager", "Retrieving available fonts");
    var fonts = _fontFamilyMap.Keys.OrderBy(f => f).ToList();
     Logger.Info("FontManager", $"Found {fonts.Count} available fonts");
    return fonts;
            }
 catch (Exception ex)
            {
        Logger.Error("FontManager", "Failed to retrieve available fonts", ex);
        return new List<string> { "SF Pro" }; // Return default font as fallback
    }
        }

        /// <summary>
        /// Gets the FontFamily resource URI for a given font name.
        /// </summary>
  /// <param name="fontName">The display name of the font (e.g., "Inter", "SF Pro")</param>
        /// <returns>The pack URI to the font resource</returns>
     public static string GetFontUri(string fontName)
     {
            try
            {
      if (string.IsNullOrWhiteSpace(fontName))
    {
  Logger.Warning("FontManager", "Null or empty font name provided, using default");
         fontName = "SF Pro";
          }

       if (_fontFamilyMap.TryGetValue(fontName, out var fontPath))
       {
       var uri = $"pack://application:,,,/MySchool;component/resources/fonts/{fontPath}";
        Logger.Debug("FontManager", $"Font URI for '{fontName}': {uri}");
          return uri;
           }

 Logger.Warning("FontManager", $"Font '{fontName}' not found in map, using default SF Pro");
      return $"pack://application:,,,/MySchool;component/resources/fonts/{_fontFamilyMap["SF Pro"]}";
     }
        catch (Exception ex)
   {
         Logger.Error("FontManager", $"Failed to get font URI for '{fontName}'", ex);
         return $"pack://application:,,,/MySchool;component/resources/fonts/{_fontFamilyMap["SF Pro"]}";
          }
        }

   /// <summary>
      /// Creates a FontFamily object for the given font name.
        /// </summary>
        /// <param name="fontName">The display name of the font</param>
        /// <returns>A FontFamily object for the specified font</returns>
        public static FontFamily GetFontFamily(string fontName)
      {
   try
            {
 var uri = GetFontUri(fontName);
            var fontFamily = new FontFamily(uri);
          Logger.Debug("FontManager", $"Created FontFamily for '{fontName}'");
 return fontFamily;
            }
catch (Exception ex)
          {
          Logger.Error("FontManager", $"Failed to create FontFamily for '{fontName}'", ex);
 // Return default system font as ultimate fallback
 return new FontFamily("Segoe UI");
            }
        }

        /// <summary>
        /// Validates if a font name exists in the available fonts.
        /// </summary>
        /// <param name="fontName">The font name to validate</param>
        /// <returns>True if the font exists, false otherwise</returns>
        public static bool IsFontAvailable(string fontName)
        {
 var exists = _fontFamilyMap.ContainsKey(fontName);
     Logger.Debug("FontManager", $"Font '{fontName}' availability: {exists}");
            return exists;
 }
    }
}
