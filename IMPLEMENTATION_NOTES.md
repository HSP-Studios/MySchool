# Navigation Bar with Mica Background Implementation

## Overview
This implementation adds a modern navigation bar interface with Mica backdrop effect for Windows 11.

## Features Implemented

### 1. Navigation Bar
- **Left sidebar navigation** with the following pages:
  - Home
  - Chat
  - Schedule
  - Resources
- Uses RadioButton controls for navigation items
- Selected state is automatically managed

### 2. Mica Background Effect
- Applied using Windows 11 DWM (Desktop Window Manager) APIs
- Provides translucent, blurred background that adapts to desktop wallpaper
- Falls back gracefully on older Windows versions
- Uses ModernWPF library for consistent modern UI styling

### 3. Page Structure
- Each navigation item has a dedicated UserControl page:
  - `HomePage.xaml` - Welcome page
  - `ChatPage.xaml` - Messaging interface placeholder
  - `SchedulePage.xaml` - Schedule view placeholder
  - `ResourcesPage.xaml` - Resources view placeholder

### 4. Custom Window Chrome
- Custom title bar with application name
- Minimize, Maximize, and Close buttons
- Transparent background to allow Mica effect to show through

## Technical Details

### Dependencies Added
- **ModernWpfUI** (v0.9.6) - Modern styling and theme support
- **Microsoft.Windows.SDK.Contracts** (v10.0.22621.3233) - Windows 11 API support

### Target Framework
Updated from `net9.0-windows` to `net9.0-windows10.0.22621.0` to enable Windows 11 features.

### Key Files Modified
1. **MySchool.csproj** - Added NuGet packages and updated target framework
2. **App.xaml** - Added ModernWPF theme resources
3. **MainWindow.xaml** - Complete redesign with navigation bar
4. **MainWindow.xaml.cs** - Added navigation logic and Mica backdrop implementation

### Key Files Created
- `Pages/HomePage.xaml` and `.xaml.cs`
- `Pages/ChatPage.xaml` and `.xaml.cs`
- `Pages/SchedulePage.xaml` and `.xaml.cs`
- `Pages/ResourcesPage.xaml` and `.xaml.cs`

## How to Test

### Requirements
- Windows 11 (for Mica effect)
- .NET 9.0 SDK
- Visual Studio 2022 or later (recommended)

### Build and Run
```bash
dotnet restore
dotnet build
dotnet run --project MySchool/MySchool.csproj
```

Or open the solution in Visual Studio and press F5.

### Expected Behavior
1. Application opens with a translucent Mica background
2. Home page is displayed by default
3. Clicking navigation items switches between pages
4. Window controls (minimize, maximize, close) work correctly
5. Mica effect shows through the navigation bar and content areas

### Fallback Behavior
On Windows 10 or if Mica is not available:
- Application will display with standard opaque background
- All functionality remains intact
- ModernWPF theme still provides modern styling

## Future Enhancements
- Add content to each page (forms, data grids, etc.)
- Implement actual chat functionality
- Add calendar/schedule components
- Include resource management features
- Add settings page with theme switching
- Implement responsive design for different window sizes
