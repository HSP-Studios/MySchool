<h1> <p "font-size:200px;"> <img align="left" src="https://github.com/HSP-Studios/MySchool/blob/main/MySchool/resources/logo/png/Dark-Icon.png" alt="" width="125">MySchool</p> </h1>

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-00A4EF?logo=windows11)](https://www.microsoft.com/windows)
<br><br>

### 🎓 Modern Student Information Management System
  
*A sleek, intuitive WPF application for displaying student information and resources at a glance.*

[Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Architecture](#%EF%B8%8F-architecture) • [Contributing](#-contributing) • [Branch Layout](#-branch-layout)

---

## 📖 About

**MySchool** is a modern Windows desktop application built with WPF (Windows Presentation Foundation) that helps students efficiently manage their student schedules, class information, and school calendars. With a beautiful, user-friendly interface featuring dark mode support and smooth navigation, MySchool makes educational data management effortless.

### 🌏 Regional Support

> **Note:** Currently, MySchool supports **Queensland, Australia** school calendars and term dates (2025-2029).
> 
> 📬 Want support for your region? [Create an issue](https://github.com/HSP-Studios/MySchool/issues/new?template=region_support.yml) and we'll implement it!

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🏠 **Home Dashboard** | Quick overview of current term, upcoming holidays, and key information |
| 📅 **Schedule Management** | Organize and view class schedules with intuitive navigation |
| ⚙️ **Settings & Customization** | Personalize your experience with theme options |
| 🌙 **Dark Mode Support** | Easy on the eyes with automatic theme switching |
| 🎨 **Modern UI/UX** | Clean, professional interface using SF Pro font and contemporary design patterns |
| 📊 **Term & Holiday Tracking** | Built-in Queensland school term dates and holiday calendars (2025-2029) |
| 💾 **Persistent Settings** | User preferences saved locally in AppData |
| 🖼️ **Custom Branding** | Professional logo assets in multiple formats (SVG, PNG) |

---

## 📂 Project Structure

```
MySchool/
├── 📄 MySchool.sln              # Visual Studio solution file
├── 📄 README.md                 # This file
├── 📄 LICENSE                   # AGPL v3 License
└── MySchool/                    # Main application project
    ├── 📁 Classes/              # Core application logic
    │   ├── HolidayLogic.cs      # School holiday calculations
    │   ├── SettingsService.cs   # User settings persistence
    │   └── ThemeManager.cs      # Dynamic theme management
    ├── 📁 Pages/                # Application pages (WPF Pages)
    │   ├── Home.xaml/cs         # Home dashboard page
    │   ├── Schedule.xaml/cs     # Schedule management page
    │   └── Settings.xaml/cs     # Settings & preferences page
    ├── 📁 resources/            # Application assets
    │   ├── 📁 data/
    │   │   └── holidays/
    │   │       └── QLD.json     # Queensland school calendar data
    │   ├── 📁 fonts/
    │   │   └── SF-Pro.ttf       # Apple SF Pro font
    │   └── 📁 logo/             # Branding assets
    │       ├── png/             # PNG logos (with background)
    │       ├── png-transparent/ # PNG logos (transparent)
    │       ├── svg/             # SVG logos (with background)
    │       └── svg-transparent/ # SVG logos (transparent)
    ├── App.xaml/cs              # Application startup & global resources
    ├── MainWindow.xaml/cs       # Main application window & navigation
    ├── AssemblyInfo.cs          # Assembly metadata
    └── MySchool.csproj          # Project configuration file
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

### Dark Mode

Toggle dark mode in **Settings**:
- Switches between light and dark color schemes
- Preference is saved and persists across sessions
- Dynamic color palette updates instantly

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
│           Page Navigation             │
│  ┌─────────┐ ┌──────────┐ ┌────────┐  │
│  │  Home   │ │ Schedule │ │Settings│  │
│  └─────────┘ └──────────┘ └────────┘  │
└──────────────┬────────────────────────┘
               │ Uses Services
               ▼
┌──────────────────────────────────────┐
│         Business Logic               │
│  • ThemeManager (Dynamic Theming)    │
│  • SettingsService (Persistence)     │
│  • HolidayLogic (Calendar Logic)     │
└──────────────────────────────────────┘
```

### Design Principles

- **🎨 Material Design Inspired**: Modern, clean UI with cards, shadows, and contemporary color palettes
- **🌐 Dynamic Resources**: Theme colors and styles defined globally and updated at runtime
- **💾 JSON-based Configuration**: Lightweight, human-readable data storage
- **🎯 Single Responsibility**: Each class has a focused purpose (theme management, settings, etc.)
- **♿ Accessibility**: Uses system fonts and proper contrast ratios

### Custom UI Components

- **Custom Window**: Frameless window with custom title bar and controls
- **Themed Scrollbars**: Rounded, modern scrollbars matching the application theme
- **Icon Buttons**: Using Segoe MDL2 Assets for consistent Windows-native icons
- **Smooth Navigation**: Frame-based page transitions without flickering

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
  <TargetFramework>net9.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

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
| **`releases/*`** | Release Source Code | Contains source code for specific releases. Format: `releases/[version]` (e.g., `releases/prototype-1`) |
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

- **SF Pro Font**: © Apple Inc. (Used for educational purposes)
- **Segoe MDL2 Assets**: © Microsoft Corporation (System font)

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
- [ ] Quick Notes
- [ ] Report predictor
- [ ] Multi-region support (NSW, VIC, SA, etc.)
- [ ] Export/import functionality
- [ ] Cloud synchronization

---

## 🙏 Acknowledgments

Special thanks to:
- The .NET and WPF communities for excellent documentation
- Contributors and testers who help improve MySchool
- Queensland Department of Education for public school calendar data

---

<div align="center">
  
  **⭐ Star this repository if you find it useful!**
  
  Made with ❤️ by HSP Studios
  
  [⬆ Back to Top](#myschool)
  
</div>
