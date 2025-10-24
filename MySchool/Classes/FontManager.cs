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
// Base URI for all fonts
        private const string FontsBaseUri = "pack://application:,,,/MySchool;component/resources/fonts/";

        private static readonly Dictionary<string, string> _fontFamilyMap = new()
        {
     { "SF Pro", "./#SF Pro" },
      { "Bitcount Grid Single", "./Bitcount_Grid_Single/#Bitcount Grid Single" },
            { "Creepster", "./Creepster/#Creepster" },
            { "Inter", "./Inter/#Inter" },
      { "League Spartan", "./League_Spartan/#League Spartan" },
        { "Montserrat", "./Montserrat/#Montserrat" },
 { "Nunito", "./Nunito/#Nunito" },
            { "Playwrite AU TAS", "./Playwrite_AU_TAS/#Playwrite AU TAS" },
            { "Press Start 2P", "./Press_Start_2P/#Press Start 2P" },
            { "Public Sans", "./Public_Sans/#Public Sans" },
  { "Roboto", "./Roboto/#Roboto" },
      { "Sanchez", "./Sanchez/#Sanchez" },
     { "Sofia Sans", "./Sofia_Sans/#Sofia Sans" },
   { "Ubuntu", "./Ubuntu/#Ubuntu" },
    { "Zalando Sans Expanded", "./Zalando_Sans_Expanded/#Zalando Sans Expanded" }
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
      var uri = FontsBaseUri + fontPath;
                    Logger.Debug("FontManager", $"Font URI for '{fontName}': {uri}");
       return uri;
      }

         Logger.Warning("FontManager", $"Font '{fontName}' not found in map, using default SF Pro");
     return FontsBaseUri + _fontFamilyMap["SF Pro"];
    }
  catch (Exception ex)
    {
    Logger.Error("FontManager", $"Failed to get font URI for '{fontName}'", ex);
       return FontsBaseUri + _fontFamilyMap["SF Pro"];
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
        Logger.Debug("FontManager", $"Created FontFamily for '{fontName}' - Source: {fontFamily.Source}");
             
           // Try to verify the font can actually render
        try
      {
   var typefaces = fontFamily.GetTypefaces();
         var count = typefaces.Count();
      Logger.Debug("FontManager", $"Font '{fontName}' has {count} typeface(s)");
         
if (count == 0)
     {
       Logger.Warning("FontManager", $"Font '{fontName}' has no typefaces, may not render correctly");
           }
    }
         catch (Exception ex)
      {
        Logger.Warning("FontManager", $"Could not enumerate typefaces for '{fontName}'", ex);
        }

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
