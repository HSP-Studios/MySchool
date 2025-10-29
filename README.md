<h1> <p "font-size:200px;"> <img align="left" src="https://github.com/HSP-Studios/MySchool/blob/main/MySchool/resources/logo/png/Dark-Icon.png" alt="" width="125">MySchool</p> </h1>

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-00A4EF?logo=windows11)](https://www.microsoft.com/windows)
<br><br>

### 🎓 Modern Student Information Management System
  
*A sleek, intuitive WPF application for displaying student information and resources at a glance.*

[Getting Started](#-getting-started) • [Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Architecture](#%EF%B8%8F-architecture) • [Contributing](#-contributing) • [Branch Layout](#-branch-layout)

---

## 📖 About

**MySchool** is a modern Windows desktop application built with WPF (Windows Presentation Foundation) that helps students efficiently manage their student schedules, class information, and school calendars. With a beautiful, user-friendly interface featuring dark mode support and smooth navigation, MySchool makes educational data management effortless.

### 🌏 Regional Support

> **Note:** Currently, MySchool supports **Queensland, Australia** school calendars and term dates (2025-2029).
> 
> 📬 Want support for your region? [Create an issue](https://github.com/HSP-Studios/MySchool/issues/new?template=region_support.yml) and we'll implement it!

---

## 🚀 Getting Started

New to MySchool? Follow this step-by-step guide to get up and running in minutes!

### Step 1: Check System Requirements

Before you begin, ensure your system meets these requirements:

- **Operating System**: Windows 10 or Windows 11 (64-bit)

That's it! The pre-built releases are self-contained and include everything you need.

> **Note for Developers**: If you plan to build from source, you'll also need the [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Step 2: Download and Install MySchool

1. Go to the [Releases](https://github.com/HSP-Studios/MySchool/releases) page
2. Download the latest `MySchool-win-Setup.exe` installer
3. Run the installer and follow the on-screen instructions
4. Launch MySchool from your Start Menu or desktop shortcut

> **For Developers**: If you want to build from source or use Visual Studio, see the [Installation](#-installation) section below.

### Step 3: First Launch Setup

When you launch MySchool for the first time:

1. **Welcome Screen**: The app opens to the Home page with a default greeting
2. **Default Settings**: User preferences are automatically created in `%AppData%/MySchool/user_settings.json`
3. **Light Theme**: The application starts in light mode by default

### Step 4: Personalize Your Experience

Make MySchool your own with these quick customization steps:

#### Set Your Name

1. Click the **Settings** tab (gear icon on the left sidebar)
2. Enter your name in the "Your Name" text field
3. Your Home page will now display a personalized greeting like "Good Morning, [Your Name]!"

#### Choose Your Theme

1. In **Settings**, toggle the **Dark Mode** switch
2. Watch the smooth 0.5-second transition between light and dark themes
3. Your preference is automatically saved for future sessions

### Step 5: Add Your Class Schedule

Get the most out of MySchool by uploading your timetable:

1. Click the **Schedule** tab (calendar icon on the left sidebar)
2. Click the **"Upload Timetable"** button
3. Follow the AI-powered upload wizard to process your schedule document
4. If any subject names are over 8 characters, you'll be prompted to create shorter versions
5. Your timetable is now active!

Once uploaded, your Home page will display:
- **Current Class**: What class you're in right now
- **Next Class**: Your upcoming class
- **Time Remaining**: Countdown until your current period ends

### Step 6: Explore Additional Features

Now that you're set up, explore these features:

- **Weekend Weather**: On Saturdays and Sundays, your Home page shows current weather conditions
- **PDF Viewer**: View your uploaded timetable as a PDF in the Schedule tab
- **Holiday Tracking**: MySchool knows Queensland school term dates and holidays (2025-2029)
- **Auto-Updates**: The app automatically checks for and installs updates on startup

### Need Help?

- 📖 Check the full [Usage](#-usage) section below for detailed feature documentation
- 🐛 Found a bug? [Report it here](https://github.com/HSP-Studios/MySchool/issues/new?template=bug_report.md)
- 💡 Have an idea? [Suggest a feature](https://github.com/HSP-Studios/MySchool/issues/new?template=feature_request.md)
- 💬 Questions? Visit our [Discussions](https://github.com/HSP-Studios/MySchool/discussions)

**Congratulations! You're all set to use MySchool.** 🎉

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🏠 **Home Dashboard** | Quick overview with personalized greeting, current class, next class, time remaining, and weather information |
| 📅 **Schedule Management** | Organize and view class schedules with PDF viewer support and AI-powered timetable upload |
| ⚙️ **Settings & Customization** | Personalize with your name, theme options, build info, and more |
| 🌙 **Dark Mode Support** | Smooth animated theme transitions between light and dark modes |
| 🎨 **Modern UI/UX** | Clean, professional interface using Inter font with improved button and TextBox styles |
| 📊 **Term & Holiday Tracking** | Built-in Queensland school term dates and holiday calendars (2025-2029) |
| ☀️ **Weather Integration** | Real-time weather display on weekends with geolocation support |
| 📚 **Timetable Management** | Upload and process timetables with automatic period tracking and current class detection |
| ✂️ **Subject Name Shortener** | Automatic dialog for shortening subject names over 8 characters |
| 📍 **Location Services** | Automatic location detection with manual override option for weather |
| 🔢 **Build Information** | Auto-generated build numbers and version display in Settings |
| 💾 **Persistent Settings** | User preferences saved locally in AppData |
| 🖼️ **Custom Branding** | Professional logo assets in multiple formats (SVG, PNG, ICO) |
| 📝 **Comprehensive Logging** | Date-based log files with automatic cleanup (keeps last 7 days) stored in AppData |
| 🔄 **Auto-Update System** | Built-in update checker with GitHub integration for seamless app updates |
| ⚠️ **Error Handling** | User-friendly error dialogs with quick access to log files for troubleshooting |

---

## 📂 Project Structure

```
MySchool/
├── 📄 MySchool.sln                    # Visual Studio solution file
├── 📄 README.md                       # This file
├── 📄 LICENSE                         # AGPL v3 License
└── MySchool/                          # Main application project
    ├── 📁 Classes/                    # Core application logic
    │   ├── BuildInfoHelper.cs         # Build version and date information
    │   ├── HolidayLogic.cs            # School holiday calculations
    │   ├── Logger.cs                  # Centralized logging utility with file persistence
    │   ├── PageTransition.cs          # Animated page navigation
    │   ├── SettingsService.cs         # User settings persistence
    │   ├── ThemeManager.cs            # Dynamic theme management with animations
    │   ├── TimetableManager.cs        # Timetable file handling and period tracking
    │   ├── TimetableModels.cs         # Data models for timetable structure
    │   └── WeatherService.cs          # Weather and geolocation services
    ├── 📁 Pages/                      # Application pages (WPF Pages)
    │   ├── Home.xaml/cs               # Home dashboard with weather and class info
    │   ├── Schedule.xaml/cs           # Schedule management with PDF viewer
    │   └── Settings.xaml/cs           # Settings, preferences, and build info
    ├── 📁 Windows/                    # Dialog windows
    │   ├── ErrorDialog.xaml/cs        # Error dialog with log folder access
    │   ├── ManualLocationDialog.xaml/cs      # Manual location entry
    │   ├── SubjectShortenerDialog.xaml/cs    # Subject name shortening
    │   └── TimetableUploadDialog.xaml/cs     # Timetable upload & processing
    ├── 📁 resources/                  # Application assets
    │   ├── 📁 data/
    │   │   └── holidays/
    │   │       └── QLD.json       # Queensland school calendar data
    │   ├── 📁 fonts/
    │   │   └── Inter/    # Inter font family
    │ │       └── Inter.ttc          # Inter font file
    │   ├── 📁 logo/   # Branding assets
    │   │   ├── ico/                   # Application icons
    │   │   ├── png/                   # PNG logos (with background)
    │   │   ├── png-transparent/       # PNG logos (transparent)
    │   │   ├── svg/                   # SVG logos (with background)
    │   │   └── svg-transparent/       # SVG logos (transparent)
    │   └── 📁 prompts/                # AI prompts for timetable generation
    ├── 📄 App.xaml/cs                 # Application startup & global resources
    ├── 📄 MainWindow.xaml/cs          # Main application window & navigation
    ├── 📄 AssemblyInfo.cs             # Assembly metadata
    ├── 📄 Directory.Build.targets     # Build number generation
    └── 📄 MySchool.csproj             # Project configuration file
```

## 🚀 Installation

### Prerequisites

- **Operating System**: Windows 10/11 (64-bit)
- **Framework**: [.NET 9.0 SDK or Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)
- **IDE** (for development): [Visual Studio 2022](https://visualstudio.microsoft.com/) with .NET Desktop Development workload

### Quick Start

#### Option 1: Download Pre-built Release
1. Navigate to the [Releases](https://github.com/HSP-Studios/MySchool/releases) page
2. Download the latest `MySchool-v*.exe` self-extracting archive

#### Option 2: Build from Source

```bash
# Clone the repository
git clone https://github.com/HSP-Studios/MySchool.git
cd MySchool

# Build the solution
dotnet build MySchool.sln --configuration Release

# Run the application
dotnet run --project MySchool/MySchool.csproj
```

#### Option 3: Visual Studio

1. Open `MySchool.sln` in Visual Studio 2022
2. Set `MySchool` as the startup project
3. Press `F5` or click **Start** to build and run

---

## 💻 Usage

### First Launch

When you first launch MySchool:
1. The application opens to the **Home** page
2. Default settings are created in `%AppData%/MySchool/user_settings.json`
3. Light theme is enabled by default

### Key Features

#### 🌙 Dark Mode
Toggle dark mode in **Settings**:
- Smooth 0.5s cross-fade animation between themes
- Preference is saved and persists across sessions
- Improved light theme contrast for better readability

#### 👤 Personalized Greeting
Set your name in **Settings**:
- Home page displays personalized greeting based on time of day
- Falls back to time-based greetings if no name is set
- Greetings: "Good Morning", "Good Afternoon", "Good Evening"

#### 📚 Timetable Management
Upload and manage your class schedule:
1. Click "Upload Timetable" in **Schedule** tab
2. AI processes your timetable document
3. Automatic subject name shortening for names over 8 characters
4. View current and next class on Home page
5. PDF viewer for uploaded timetables

#### ☀️ Weather Display
On weekends, the Home page shows:
- Current weather conditions and temperature
- Your location (auto-detected or manually set)
- Weather-based background gradient

#### 🔢 Build Information
View app version details in **Settings**:
- Version number (e.g., 1.0.0-beta7)
- Auto-generated build number (format: YYMMDDRRRR)
- Build date

#### 🔄 Auto-Update System
MySchool automatically checks for updates on startup:
- Seamlessly downloads and installs updates from GitHub
- Supports prerelease versions (beta updates)
- Comprehensive error handling with detailed logging
- Restart prompted after successful update installation

### Navigation

| Tab | Shortcut | Description |
|-----|----------|-------------|
| **Home** | Click "Home" tab or schedule card | View dashboard with personalized greeting, current/next class, time remaining, and weekend weather |
| **Schedule** | Click "Schedule" tab | Upload timetables, view PDF schedules, and manage class information |
| **Settings** | Click "Settings" tab | Set your name, toggle dark mode, and view build information |

#### 📝 Logging & Error Handling
MySchool includes comprehensive logging:
- Date-based log files stored in `%AppData%/MySchool/logs/`
- Automatic cleanup of logs older than 7 days
- Error dialogs with "Open Logs Folder" button for easy troubleshooting
- Detailed logging of updates, errors, warnings, and debug information

### Window Controls

- **Minimize**: Click the minimize button (—) in the top-right
- **Close**: Click the close button (✕) in the top-right
- **Move**: Click and drag the title bar area

Note: Window resizing has been disabled for aesthetics. If you would like resizing enabled, feel free to open an issue!

---

## 🏗️ Architecture

### Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | .NET | 9.0 |
| **UI Framework** | WPF (Windows Presentation Foundation) | 9.0 |
| **Target Platform** | Windows | net9.0-windows |
| **Language** | C# | 12.0 |
| **Markup** | XAML | - |
| **Serialization** | System.Text.Json | Built-in |
| **Web Browser** | Microsoft.Web.WebView2 | 1.0.3537.50 |
| **Auto-Update** | Velopack | 0.0.1298 |
| **Weather API** | Open-Meteo | Free weather data |
| **Geolocation** | Nominatim (OpenStreetMap) | Reverse geocoding |

### Architecture Pattern

MySchool uses a **Code-Behind Pattern** with the following structure:

```
┌─────────────────────────────────────┐
│         MainWindow.xaml             │
│  (Navigation & Window Management)   │
└──────────────┬──────────────────────┘
               │ Hosts Frame
               ▼
┌───────────────────────────────────────┐
│      Animated Page Navigation         │
│  ┌─────────┐ ┌──────────┐ ┌────────┐  │
│  │  Home   │ │ Schedule │ │Settings│  │
│  └─────────┘ └──────────┘ └────────┘  │
└──────────────┬────────────────────────┘
               │ Uses Services
               ▼
┌──────────────────────────────────────┐
│         Business Logic               │
│  • ThemeManager (Animated Theming)   │
│  • TimetableManager (Period Tracking)│
│  • WeatherService (Geolocation)      │
│  • SettingsService (Persistence)     │
│  • HolidayLogic (Calendar Logic)     │
│  • PageTransition (Animations)       │
│  • BuildInfoHelper (Version Info)    │
│  • Logger (Centralized Logging)      │
└──────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────┐
│          Dialog Windows              │
│  • TimetableUploadDialog             │
│  • SubjectShortenerDialog            │
│  • ManualLocationDialog              │
│  • ErrorDialog (with Log Access)     │
└──────────────────────────────────────┘
```

### Design Principles

- **🎨 Material Design Inspired**: Modern, clean UI with cards, shadows, and contemporary color palettes
- **🌐 Dynamic Resources**: Theme colors and styles defined globally and updated at runtime with smooth animations
- **💾 JSON-based Configuration**: Lightweight, human-readable data storage for settings and timetables
- **🎯 Single Responsibility**: Each class has a focused purpose (theme management, weather, timetables, logging, etc.)
- **♿ Accessibility**: Uses system fonts and proper contrast ratios with improved light/dark mode support
- **🔄 Real-time Updates**: Current class detection, weather updates, dynamic UI elements, and automatic app updates
- **📝 Comprehensive Logging**: Centralized logging with automatic log rotation and easy access for troubleshooting
- **⚠️ Robust Error Handling**: User-friendly error messages with detailed logging and quick log access

### Custom UI Components

- **Custom Window**: Frameless window with custom title bar and controls
- **Themed Scrollbars**: Rounded, modern scrollbars matching the application theme
- **Icon Buttons**: Using Segoe MDL2 Assets for consistent Windows-native icons
- **Smooth Animations**: Fade-in/out page transitions and cross-fade theme switching
- **Enhanced Buttons**: Primary and secondary button styles with hover/press states
- **Modern TextBoxes**: Styled input fields with consistent padding and interaction feedback
- **Dialog Windows**: Professional dialogs for timetable upload, subject shortening, and location entry
- **PDF Viewer**: Integrated WebView2 for viewing timetable PDFs

---

## 🛠️ Development

### Building the Project

```bash
# Restore dependencies
dotnet restore MySchool.sln

# Build in Debug mode
dotnet build MySchool.sln --configuration Debug

# Build in Release mode
dotnet build MySchool.sln --configuration Release
```

### Project Configuration

The application is configured in `MySchool.csproj`:

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net9.0-windows10.0.19041.0</TargetFramework>
  <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
  <UseWPF>true</UseWPF>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <Version>1.0.0-beta7</Version>
  <ApplicationIcon>resources\logo\ico\Dark-Icon.ico</ApplicationIcon>
</PropertyGroup>
```

### Build System

MySchool uses a custom build number generation system:

**`Directory.Build.targets`** generates unique build numbers in format `YYMMDDRRRR`:
- `YY`: Year (e.g., 25)
- `MM`: Month (e.g., 10)
- `DD`: Day (e.g., 19)
- `RRRR`: Random 4-digit number

Build information is auto-generated during compilation and accessible via `BuildInfoHelper.cs`.

### Adding New Resources

**Fonts:**
1. Add `.ttf` file to `resources/fonts/`
2. Set **Build Action** to `Resource` in project file
3. Reference in `App.xaml`: 
   ```xml
   <FontFamily>pack://application:,,,/MySchool;component/resources/fonts/YourFont.ttf#Font Name</FontFamily>
   ```

**Images/Logos:**
1. Add image to appropriate `resources/logo/` subdirectory
2. Set **Build Action** to `Resource`
3. Reference in XAML: `Source="resources/logo/png/Image.png"`

**Data Files:**
1. Add to `resources/data/`
2. Set **Build Action** to `Content` and **Copy to Output Directory** to `Always`
3. Access at runtime from output directory

### Logging System

MySchool includes a comprehensive logging system via the `Logger` class:

**Log Levels:**
- `Logger.Info()` - Informational messages
- `Logger.Warning()` - Warning messages with optional exception details
- `Logger.Error()` - Error messages with full stack traces
- `Logger.Debug()` - Debug messages (console only, not written to file)

**Log File Location:**
- Default: `%AppData%/MySchool/logs/myschool_YYYY-MM-DD.log`
- Fallback: `%TEMP%/myschool.log` if AppData is inaccessible

**Log Management:**
- Automatic daily log rotation
- Keeps logs for last 7 days
- Thread-safe file writing

**Usage Example:**
```csharp
Logger.Info("Updates", "Checking for updates...");
Logger.Error("Updates", "Failed to check for updates", exception);
```

### Code Style

- **Naming**: PascalCase for public members, camelCase for private fields
- **Indentation**: 4 spaces (no tabs)
- **Comments**: XML documentation for public APIs
- **XAML**: Attributes on separate lines for complex elements

---

## 🧪 Testing

> **Note**: Testing infrastructure is not yet implemented. Future versions will include:
> - Unit tests for business logic (SettingsService, ThemeManager, HolidayLogic)
> - Integration tests for page navigation
> - UI automation tests
> - A GitHub actions workflow for build verification

---

## 🤝 Contributing

We welcome contributions from the community! Here's how you can help:

### Ways to Contribute

- 🐛 **Report Bugs**: Open an issue describing the problem
- 💡 **Suggest Features**: Share your ideas for new functionality
- 📝 **Improve Documentation**: Help make our docs clearer
- 🌍 **Add Regional Support**: Contribute calendar data for your region
- 💻 **Submit Code**: Fix bugs or implement new features

### Contribution Workflow

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines

- Follow existing code style and patterns
- Test your changes thoroughly (especially UI changes)
- Update documentation for new features
- Keep commits focused and well-described
- Ensure the application builds without errors

### Adding Regional Support

To add support for a new region:

1. Create a new JSON file in `MySchool/resources/data/holidays/` (e.g., `NSW.json`)
2. Follow the structure of `QLD.json`:
   ```json
   {
     "2025": {
       "term_dates": { ... },
       "staff_development_days": { ... },
       "school_holidays": { ... }
     }
   }
   ```
3. Update `MySchool.csproj` to include the new file
4. Submit a pull request with your changes

---

## 🌳 Branch Layout

Understanding our repository's branch structure:

### Branch Types

| Branch Pattern | Purpose | Description |
|---------------|---------|-------------|
| **`main`** | Development | The primary development branch. Active development happens here. |
| **`release/*`** | Release Source Code | Contains source code for specific releases. Format: `releases/[version]` (e.g., `releases/prototype-1`) |
| **`testing/*`** | Testing & Validation | Branches for testing new features, optimizations, and experimental changes before merging to main |
| **`feature/*`** | Feature Development | Branches for developing new features (e.g., `feature/11-add-nsw-vic-sa-wa-nt-act-tas-school-holiday-lists`) |
| **`copilot/*`** | AI-Assisted Changes | Branches created by GitHub Copilot for automated improvements |

### Finding Release Source Code

Looking for source code of a specific release?
- Navigate to the `releases/` branches (e.g., `releases/prototype-1`)
- Or visit the [Releases page](https://github.com/HSP-Studios/MySchool/releases) for compiled binaries

### Branch Workflow

1. **Main**: Active work is merged into `main` branch
2. **Testing**: New features are tested in `testing/*` branches
3. **Development**: New features are developed in `feature/*` branches and later merged into `main`
4. **Bugs**: Normal bugs are worked on in `bugs/` or `bug/` branches
5. **Hotfixes**: Hotfixes or urgent bugs are worked on in `hotfix/` branches 
6. **Release**: Stable versions are tagged and source code is preserved in `releases/*` branches

---

## 📜 License

This project is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)**.

### What This Means

- ✅ You can use, modify, and distribute this software
- ✅ Source code must be made available when distributing
- ✅ Modifications must be released under the same license
- ✅ Network use counts as distribution (must provide source)
- ❌ No warranty or liability provided

See the [LICENSE](LICENSE) file for the full license text.

### Third-Party Assets

- **Inter Font**: © The Inter Project Authors (SIL Open Font License 1.1)
- **Segoe MDL2 Assets**: © Microsoft Corporation (System font)
- **Open-Meteo Weather API**: Free weather data service
- **Nominatim (OpenStreetMap)**: Reverse geocoding service
- **Microsoft WebView2**: For PDF viewing capabilities

---

## 👥 Contributors

### HSP Studios
**Development Team**
- [@HighSchoolProgrammer](https://github.com/HighSchoolProgrammer)

**Design Team**
- [@HighSchoolProgrammer](https://github.com/HighSchoolProgrammer)

**Translation Team**
- [@HighSchoolProgrammer](https://github.com/HighSchoolProgrammer)
- [@VivianZZz0](https://github.com/VivianZZz0)

**App Tester Team**
- [@HighSchoolProgrammer](https://github.com/HighSchoolProgrammer)
- [@VivianZZz0](https://github.com/VivianZZz0)

### Other Contributors
- None yet ;)

---

## 💬 Support & Contact

### Getting Help

- 📖 **Documentation**: You're reading it! Check the sections above
- 🐛 **Bug Reports**: [Open an issue](https://github.com/HSP-Studios/MySchool/issues/new?template=bug_report.md)
- 💡 **Feature Requests**: [Open an issue](https://github.com/HSP-Studios/MySchool/issues/new?template=feature_request.md)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/HSP-Studios/MySchool/discussions)

### Roadmap

Future planned features:
- [x] ~~Timetable management with AI processing~~
- [x] ~~Dark mode with smooth animations~~
- [x] ~~Weather integration~~
- [x] ~~Personalized greetings~~
- [x] ~~Build version tracking~~
- [ ] Quick Notes
- [ ] Report predictor
- [ ] Multi-region support (NSW, VIC, SA, etc.)
- [ ] Export/import functionality
- [ ] Cloud synchronization
- [ ] Mobile companion app

---

## 🙏 Acknowledgments

Special thanks to:
- The .NET and WPF communities for excellent documentation
- Contributors and testers who help improve MySchool
- Queensland Department of Education for public school calendar data
- InstallForge's easy-to-use installer creator
- Open-Meteo for free weather API access
- OpenStreetMap/Nominatim for geolocation services
- Velopack for the amazing auto-update system
- GitHub Copilot for AI-assisted development

---

<div align="center">
  
  **⭐ Star this repository if you find it useful!**
  
  Made with ❤️ by HSP Studios
  
  [⬆ Back to Top](#--myschool-)
  
</div>
