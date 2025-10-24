using System.IO;
using System.Windows;
using System.Windows.Media;

namespace MySchool.Classes
{
    /// <summary>
    /// Helper class for managing font families in the application.
    /// Loads fonts from the resources/fonts directory in the application folder.
    /// </summary>
    public static class FontManager
    {
        // Map of display names to font file paths
        private static readonly Dictionary<string, string> _fontMap = new()
     {
 { "SF Pro", "SF-Pro.ttf" },
         { "Bitcount Grid Single", "Bitcount_Grid_Single/BitcountGridSingle.ttf" },
      { "Creepster", "Creepster/Creepster-Regular.ttf" },
 { "Inter", "Inter/static/Inter_18pt-Regular.ttf" },
         { "League Spartan", "League_Spartan/static/LeagueSpartan-Regular.ttf" },
            { "Montserrat", "Montserrat/static/Montserrat-Regular.ttf" },
            { "Nunito", "Nunito/static/Nunito-Regular.ttf" },
            { "Playwrite AU TAS", "Playwrite_AU_TAS/PlaywriteAUTAS-Regular.ttf" },
 { "Press Start 2P", "Press_Start_2P/PressStart2P-Regular.ttf" },
            { "Public Sans", "Public_Sans/static/PublicSans-Regular.ttf" },
            { "Roboto", "Roboto/static/Roboto-Regular.ttf" },
            { "Sanchez", "Sanchez/Sanchez-Regular.ttf" },
            { "Sofia Sans", "Sofia_Sans/static/SofiaSans-Regular.ttf" },
            { "Ubuntu", "Ubuntu/Ubuntu-Regular.ttf" },
            { "Zalando Sans Expanded", "Zalando_Sans_Expanded/static/ZalandoSansExpanded-Regular.ttf" }
        };

 /// <summary>
     /// Gets the base directory where fonts are located (app directory/resources/fonts)
        /// </summary>
        private static string GetFontsDirectory()
        {
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
 var fontsDir = Path.Combine(appDir, "resources", "fonts");
   Logger.Debug("FontManager", $"Fonts directory: {fontsDir}");
            return fontsDir;
 }

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
          return new List<string> { "SF Pro" };
            }
        }

        /// <summary>
        /// Gets the full file path for a given font name.
        /// </summary>
      public static string GetFontPath(string fontName)
        {
      try
 {
       if (string.IsNullOrWhiteSpace(fontName))
     {
   Logger.Warning("FontManager", "Null or empty font name provided, using default");
      fontName = "SF Pro";
       }

      if (_fontMap.TryGetValue(fontName, out var fontFile))
                {
         var fontsDir = GetFontsDirectory();
     var fullPath = Path.Combine(fontsDir, fontFile);
        Logger.Debug("FontManager", $"Font path for '{fontName}': {fullPath}");
     
           // Verify file exists
      if (File.Exists(fullPath))
      {
     Logger.Debug("FontManager", $"Font file verified to exist: {fullPath}");
      return fullPath;
          }
    else
           {
    Logger.Error("FontManager", $"Font file does not exist: {fullPath}");
   throw new FileNotFoundException($"Font file not found: {fullPath}");
   }
     }

     Logger.Warning("FontManager", $"Font '{fontName}' not found in map, using default SF Pro");
        var defaultPath = Path.Combine(GetFontsDirectory(), _fontMap["SF Pro"]);
       return defaultPath;
      }
      catch (Exception ex)
            {
     Logger.Error("FontManager", $"Failed to get font path for '{fontName}'", ex);
                var defaultPath = Path.Combine(GetFontsDirectory(), _fontMap["SF Pro"]);
      return defaultPath;
   }
        }

     /// <summary>
  /// Creates a FontFamily object for the given font name.
  /// Loads from the file system.
        /// </summary>
 public static FontFamily GetFontFamily(string fontName)
        {
  try
   {
        var fontPath = GetFontPath(fontName);
         
           // Create FontFamily from file path
    // Format: file:///C:/path/to/font.ttf#FontFamilyName
           var uri = new Uri(fontPath, UriKind.Absolute);
         var fontFamily = new FontFamily(uri, fontName);
           
    Logger.Info("FontManager", $"Created FontFamily for '{fontName}' from {fontPath}");
 
     // Verify the font loads correctly
    try
     {
      var typefaces = fontFamily.GetTypefaces();
    var count = typefaces.Count();
           Logger.Info("FontManager", $"Font '{fontName}' loaded successfully with {count} typeface(s)");
         
       if (count == 0)
        {
   Logger.Warning("FontManager", $"Font '{fontName}' has no typefaces");
     throw new Exception("Font has no typefaces");
    }
      }
  catch (Exception ex)
     {
            Logger.Error("FontManager", $"Font '{fontName}' failed verification", ex);
   throw;
           }

       return fontFamily;
    }
   catch (Exception ex)
  {
    Logger.Error("FontManager", $"Failed to create FontFamily for '{fontName}', using system fallback", ex);
        Logger.Warning("FontManager", "Falling back to Segoe UI system font");
      return new FontFamily("Segoe UI");
            }
        }

        /// <summary>
      /// Validates if a font name exists in the available fonts.
        /// </summary>
        public static bool IsFontAvailable(string fontName)
      {
     var exists = _fontMap.ContainsKey(fontName);
          Logger.Debug("FontManager", $"Font '{fontName}' availability: {exists}");
  return exists;
        }
    }
}
