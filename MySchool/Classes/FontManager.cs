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

     private static string GetFontsDirectory()
        {
     Logger.Debug("FontManager", "=== GetFontsDirectory START ===");
            
       var appDir = AppDomain.CurrentDomain.BaseDirectory;
    Logger.Info("FontManager", $"AppDomain.CurrentDomain.BaseDirectory: {appDir}");
   
            var fontsDir = Path.Combine(appDir, "resources", "fonts");
            Logger.Info("FontManager", $"Computed fonts directory: {fontsDir}");
         
  bool exists = Directory.Exists(fontsDir);
            Logger.Info("FontManager", $"Fonts directory exists: {exists}");
            
            if (exists)
            {
                try
         {
               var files = Directory.GetFiles(fontsDir, "*.ttf", SearchOption.AllDirectories);
                    Logger.Info("FontManager", $"Found {files.Length} TTF files in fonts directory");
    foreach (var file in files.Take(5))
          {
                Logger.Debug("FontManager", $"  Font file: {file}");
             }
     if (files.Length > 5)
    {
      Logger.Debug("FontManager", $"  ... and {files.Length - 5} more files");
            }
    }
           catch (Exception ex)
       {
   Logger.Error("FontManager", "Failed to enumerate font files", ex);
                }
  }
         else
 {
  Logger.Error("FontManager", $"CRITICAL: Fonts directory does not exist at {fontsDir}");
   }
            
  Logger.Debug("FontManager", "=== GetFontsDirectory END ===");
  return fontsDir;
        }

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

        public static string GetFontPath(string fontName)
        {
      Logger.Debug("FontManager", $"=== GetFontPath START for '{fontName}' ===");
            
  try
  {
  if (string.IsNullOrWhiteSpace(fontName))
       {
     Logger.Warning("FontManager", "Null or empty font name provided, using default");
  fontName = "SF Pro";
      }

      Logger.Info("FontManager", $"Looking up font '{fontName}' in font map");
    
  if (_fontMap.TryGetValue(fontName, out var fontFile))
     {
              Logger.Info("FontManager", $"Font map entry found: '{fontFile}'");
       
  var fontsDir = GetFontsDirectory();
       Logger.Info("FontManager", $"Fonts directory: {fontsDir}");
        
   var fullPath = Path.Combine(fontsDir, fontFile);
        Logger.Info("FontManager", $"Full font path computed: {fullPath}");

             bool fileExists = File.Exists(fullPath);
              Logger.Info("FontManager", $"Font file exists: {fileExists}");
              
 if (fileExists)
             {
     var fileInfo = new FileInfo(fullPath);
  Logger.Info("FontManager", $"Font file size: {fileInfo.Length} bytes");
            Logger.Info("FontManager", $"Font file last modified: {fileInfo.LastWriteTime}");
        Logger.Debug("FontManager", $"=== GetFontPath END - SUCCESS ===");
   return fullPath;
               }
           else
               {
                 Logger.Error("FontManager", $"CRITICAL: Font file does not exist at: {fullPath}");
          
     var dir = Path.GetDirectoryName(fullPath);
  if (dir != null && Directory.Exists(dir))
           {
      Logger.Info("FontManager", $"Directory exists: {dir}");
  var filesInDir = Directory.GetFiles(dir);
   Logger.Info("FontManager", $"Files in directory: {filesInDir.Length}");
foreach (var file in filesInDir)
         {
        Logger.Debug("FontManager", $"  Available: {Path.GetFileName(file)}");
             }
       }
     else
  {
           Logger.Error("FontManager", $"Parent directory does not exist: {dir}");
       }
       
            throw new FileNotFoundException($"Font file not found: {fullPath}");
         }
    }

   Logger.Warning("FontManager", $"Font '{fontName}' not found in map, using default SF Pro");
          var defaultPath = Path.Combine(GetFontsDirectory(), _fontMap["SF Pro"]);
     Logger.Info("FontManager", $"Default font path: {defaultPath}");
         Logger.Debug("FontManager", $"=== GetFontPath END - DEFAULT ===");
    return defaultPath;
        }
        catch (Exception ex)
       {
          Logger.Error("FontManager", $"Exception in GetFontPath for '{fontName}'", ex);
    var defaultPath = Path.Combine(GetFontsDirectory(), _fontMap["SF Pro"]);
  Logger.Info("FontManager", $"Returning default path due to exception: {defaultPath}");
        Logger.Debug("FontManager", $"=== GetFontPath END - EXCEPTION ===");
          return defaultPath;
            }
  }

        public static FontFamily GetFontFamily(string fontName)
        {
      Logger.Debug("FontManager", $"=== GetFontFamily START for '{fontName}' ===");
 
try
            {
      Logger.Info("FontManager", $"Attempting to create FontFamily for '{fontName}'");
          
           var fontPath = GetFontPath(fontName);
       Logger.Info("FontManager", $"Font path retrieved: {fontPath}");

     // Get directory containing the font
  var directory = Path.GetDirectoryName(fontPath);
   if (directory == null)
      {
      throw new Exception("Could not determine font directory");
   }
     Logger.Info("FontManager", $"Font directory: {directory}");
 
  // Create directory URI with trailing separator
   var directoryUri = new Uri(directory + Path.DirectorySeparatorChar, UriKind.Absolute);
    Logger.Info("FontManager", $"Directory URI: {directoryUri}");
       
    // Create FontFamily using directory as base and font name as fragment
   // Format: file:///path/to/directory/#FontName
      Logger.Debug("FontManager", $"Creating FontFamily with directory URI and fragment ./#  {fontName}'");
        var fontFamily = new FontFamily(directoryUri, "./#" + fontName);
      
     Logger.Info("FontManager", $"FontFamily created successfully");
        Logger.Info("FontManager", $"FontFamily.Source: {fontFamily.Source}");
        Logger.Info("FontManager", $"FontFamily.BaseUri: {fontFamily.BaseUri}");
    Logger.Info("FontManager", $"FontFamily.FamilyNames count: {fontFamily.FamilyNames.Count}");
       
     foreach (var kvp in fontFamily.FamilyNames)
     {
 Logger.Debug("FontManager", $"  FamilyName [{kvp.Key}]: {kvp.Value}");
      }

  Logger.Debug("FontManager", "Attempting to get typefaces");
   try
            {
 var typefaces = fontFamily.GetTypefaces();
        Logger.Debug("FontManager", "GetTypefaces() succeeded");

           var typefacesList = typefaces.ToList();
         var count = typefacesList.Count;
  Logger.Info("FontManager", $"Font '{fontName}' has {count} typeface(s)");

      if (count == 0)
        {
      Logger.Error("FontManager", $"CRITICAL: Font '{fontName}' has ZERO typefaces - font will not render!");
     throw new Exception("Font has no typefaces");
   }

      for (int i = 0; i < Math.Min(count, 3); i++)
   {
     var tf = typefacesList[i];
         Logger.Info("FontManager", $"  Typeface {i}: Weight={tf.Weight}, Style={tf.Style}, Stretch={tf.Stretch}");
       
         try
      {
 var glyphTypeface = tf.TryGetGlyphTypeface(out var glyph);
  Logger.Debug("FontManager", $"    TryGetGlyphTypeface: {glyphTypeface}");
        if (glyphTypeface && glyph != null)
         {
         Logger.Debug("FontManager", $"    GlyphCount: {glyph.GlyphCount}");
        Logger.Debug("FontManager", $"    DesignerName: {glyph.Copyrights?.FirstOrDefault().Value}");
      }
        }
         catch (Exception ex)
     {
           Logger.Warning("FontManager", $"    Failed to get glyph typeface details", ex);
        }
        }

        Logger.Info("FontManager", $"? Font '{fontName}' loaded and verified successfully");
      Logger.Debug("FontManager", $"=== GetFontFamily END - SUCCESS ===");
      return fontFamily;
       }
  catch (Exception ex)
       {
      Logger.Error("FontManager", $"Font '{fontName}' failed typeface verification", ex);
        Logger.Error("FontManager", $"Exception type: {ex.GetType().Name}");
      Logger.Error("FontManager", $"Exception message: {ex.Message}");
      if (ex.InnerException != null)
     {
   Logger.Error("FontManager", $"Inner exception: {ex.InnerException.Message}");
   }
     throw;
     }
       }
     catch (Exception ex)
   {
        Logger.Error("FontManager", $"? Failed to create FontFamily for '{fontName}'", ex);
    Logger.Error("FontManager", $"Exception type: {ex.GetType().Name}");
      Logger.Error("FontManager", $"Exception message: {ex.Message}");
  Logger.Error("FontManager", $"Stack trace: {ex.StackTrace}");

      Logger.Warning("FontManager", "Falling back to Segoe UI system font");
 var fallback = new FontFamily("Segoe UI");
        Logger.Info("FontManager", $"Fallback font created: {fallback.Source}");
 Logger.Debug("FontManager", $"=== GetFontFamily END - FALLBACK ===");
       return fallback;
   }
        }

      public static bool IsFontAvailable(string fontName)
     {
            var exists = _fontMap.ContainsKey(fontName);
            Logger.Debug("FontManager", $"Font '{fontName}' availability check: {exists}");
    return exists;
   }
    }
}
