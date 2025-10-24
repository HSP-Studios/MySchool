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
 // Map of display names to font file paths and family names
        private static readonly Dictionary<string, (string path, string family)> _fontMap = new()
    {
   { "SF Pro", ("SF-Pro.ttf", "SF Pro") },
            { "Bitcount Grid Single", ("Bitcount_Grid_Single/BitcountGridSingle.ttf", "Bitcount Grid Single") },
            { "Creepster", ("Creepster/Creepster-Regular.ttf", "Creepster") },
           { "Inter", ("Inter/static/Inter_18pt-Regular.ttf", "Inter 18pt") },
          { "League Spartan", ("League_Spartan/static/LeagueSpartan-Regular.ttf", "League Spartan") },
            { "Montserrat", ("Montserrat/static/Montserrat-Regular.ttf", "Montserrat") },
       { "Nunito", ("Nunito/static/Nunito-Regular.ttf", "Nunito") },
            { "Playwrite AU TAS", ("Playwrite_AU_TAS/PlaywriteAUTAS-Regular.ttf", "Playwrite AU TAS") },
 { "Press Start 2P", ("Press_Start_2P/PressStart2P-Regular.ttf", "Press Start 2P") },
            { "Public Sans", ("Public_Sans/static/PublicSans-Regular.ttf", "Public Sans") },
            { "Roboto", ("Roboto/static/Roboto-Regular.ttf", "Roboto") },
       { "Sanchez", ("Sanchez/Sanchez-Regular.ttf", "Sanchez") },
            { "Sofia Sans", ("Sofia_Sans/static/SofiaSans-Regular.ttf", "Sofia Sans") },
            { "Ubuntu", ("Ubuntu/Ubuntu-Regular.ttf", "Ubuntu") },
{ "Zalando Sans Expanded", ("Zalando_Sans_Expanded/static/ZalandoSansExpanded-Regular.ttf", "Zalando Sans Expanded") }
     };

      /// <summary>
     /// Gets a list of all available font family names.
        /// </summary>
    public static List<string> GetAvailableFonts()
     {
            try
            {
                Logger.Debug("FontManager", "Retrieving available fonts");
    var fonts = _fontMap.Keys.OrderBy(f => f).ToList();
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
        /// <returns>The complete font family reference string</returns>
     public static string GetFontUri(string fontName)
        {
 try
  {
       if (string.IsNullOrWhiteSpace(fontName))
         {
    Logger.Warning("FontManager", "Null or empty font name provided, using default");
  fontName = "SF Pro";
     }

       if (_fontMap.TryGetValue(fontName, out var fontInfo))
              {
        // Use format: pack://application:,,,/MySchool;component/path/to/font.ttf#FontFamilyName
            var uri = $"pack://application:,,,/MySchool;component/resources/fonts/{fontInfo.path}#{fontInfo.family}";
    Logger.Debug("FontManager", $"Font URI for '{fontName}': {uri}");
            return uri;
   }

    Logger.Warning("FontManager", $"Font '{fontName}' not found in map, using default SF Pro");
        var defaultInfo = _fontMap["SF Pro"];
            return $"pack://application:,,,/MySchool;component/resources/fonts/{defaultInfo.path}#{defaultInfo.family}";
   }
  catch (Exception ex)
     {
       Logger.Error("FontManager", $"Failed to get font URI for '{fontName}'", ex);
         var defaultInfo = _fontMap["SF Pro"];
     return $"pack://application:,,,/MySchool;component/resources/fonts/{defaultInfo.path}#{defaultInfo.family}";
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
      Logger.Info("FontManager", $"Created FontFamily for '{fontName}' - Source: {fontFamily.Source}");
       
                // Verify the font
                try
          {
            var typefaces = fontFamily.GetTypefaces();
               var count = typefaces.Count();
    Logger.Debug("FontManager", $"Font '{fontName}' has {count} typeface(s)");
     
  if (count == 0)
 {
      Logger.Warning("FontManager", $"Font '{fontName}' has no typefaces - font may not be embedded correctly");
    throw new Exception($"Font has no typefaces");
}
           
     // Log first typeface details
              var firstTypeface = typefaces.FirstOrDefault();
      if (firstTypeface != null)
   {
               Logger.Debug("FontManager", $"First typeface - Weight: {firstTypeface.Weight}, Style: {firstTypeface.Style}, Stretch: {firstTypeface.Stretch}");
       }
  }
    catch (Exception ex)
      {
     Logger.Error("FontManager", $"Font '{fontName}' failed typeface verification", ex);
              throw; // Re-throw to use fallback
       }

     return fontFamily;
   }
     catch (Exception ex)
   {
        Logger.Error("FontManager", $"Failed to create FontFamily for '{fontName}', using system fallback", ex);
          // Return default system font as ultimate fallback
Logger.Warning("FontManager", "Falling back to Segoe UI system font");
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
  var exists = _fontMap.ContainsKey(fontName);
         Logger.Debug("FontManager", $"Font '{fontName}' availability: {exists}");
            return exists;
        }
    }
}
