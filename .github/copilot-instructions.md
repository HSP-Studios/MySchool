# Copilot Instructions for MySchool

## Project Overview
MySchool is a modern WPF application for managing and displaying student information, schedules, and school calendars. It features a clean, dark-mode-capable UI with navigation between Home, Schedule, and Settings pages. The main UI and logic reside in `MainWindow.xaml` and `MainWindow.xaml.cs`. Application-level configuration is handled in `App.xaml` and `App.xaml.cs`.

**Current Regional Support:** Queensland, Australia (2025-2029 school calendar data).

For comprehensive project information, see [README.md](../README.md).

## Architecture & Key Files
- `MainWindow.xaml` / `MainWindow.xaml.cs`: Main window UI and code-behind. Entry point for user interaction with frameless window and navigation.
- `Pages/`: Contains UI pages for navigation:
  - `Home.xaml/cs`: Dashboard showing current term and upcoming holidays
  - `Schedule.xaml/cs`: Class schedule management
  - `Settings.xaml/cs`: User preferences and theme toggle
- `Classes/`: Contains business logic classes:
  - `ThemeManager.cs`: Dynamic theme switching (light/dark mode)
  - `SettingsService.cs`: User settings persistence (JSON)
  - `HolidayLogic.cs`: School calendar and term date calculations
- `App.xaml` / `App.xaml.cs`: Application startup, global resources, and event handling.
- `resources/`: Contains assets organized by type:
  - `data/holidays/`: School calendar JSON files (e.g., QLD.json)
  - `fonts/`: Typography assets (SF-Pro.ttf)
  - `logo/`: Branding assets in multiple formats and variants
- `MySchool.csproj`: Project configuration, target framework (net9.0-windows), resource definitions.

## Developer Workflows
- **Build:** Use Visual Studio or run `dotnet build MySchool.csproj` from the MySchool project directory. **Requires Windows OS or EnableWindowsTargeting=true.**
- **Run:** Launch via Visual Studio or `dotnet run --project MySchool.csproj`.
- **Debug:** Preferred in Visual Studio for WPF UI debugging with breakpoints.
- **Assets:** Reference images and fonts from the `resources/` directory using relative paths in XAML.
- **Testing:** No automated tests currently exist. Manual testing via running the application is required.

### Adding New Assets
- **Fonts**: Add `.ttf` files to `resources/fonts/`, set Build Action to `Resource`, reference in App.xaml.
- **Images/Logos**: Add to `resources/logo/` subdirectories (png/, png-transparent/, svg/, svg-transparent/), set Build Action to `Resource`.
- **Data Files**: Add JSON files to `resources/data/`, set Build Action to `Content`, Copy to Output Directory to `Always`.

## Patterns & Conventions
- Code-behind pattern is used for UI logic (no MVVM or data binding frameworks detected).
- UI resources (images, fonts) are organized under `resources/` with subfolders for format and transparency.
- Project targets Windows only (`net9.0-windows`). **Build requires Windows OS or EnableWindowsTargeting property.**
- No custom build scripts or external service integrations detected.
- Services pattern: `ThemeManager`, `SettingsService`, and `HolidayLogic` classes provide business logic.
- Settings are persisted in JSON format at `%AppData%/MySchool/user_settings.json`.
- Holiday/calendar data is loaded from `resources/data/holidays/*.json` files.

## Examples
- To display a logo in XAML:
  ```xml
  <Image Source="resources/logo/png/Dark-Icon.png" Width="125" />
  ```
- To handle application startup logic, use `App.xaml.cs`.
- To reference SF Pro font in XAML:
  ```xml
  <TextBlock FontFamily="pack://application:,,,/MySchool;component/resources/fonts/SF-Pro.ttf#SF Pro Display" />
  ```
- To navigate to a page in MainWindow code-behind:
  ```csharp
  ContentFrame.Navigate(new Pages.Home());
  ```

## Recommendations for AI Agents
- Maintain resource organization when adding new assets.
- Use .NET and WPF conventions for UI and event handling.
- If adding new features, follow the existing code-behind approach unless refactoring to MVVM is explicitly requested.
- When adding regional support, create new JSON files in `resources/data/holidays/` following the QLD.json structure.
- Test all UI changes manually by running the application (no automated UI tests exist).
- Respect the AGPL v3.0 license requirements in any code suggestions.
- Use issue templates (.github/ISSUE_TEMPLATE/) as guides for bug reports, feature requests, and regional support requests.

## Git, Branching and Pull Request Workflow (required)

- Before starting work on any user-assigned task, create a dedicated task branch from `main` using one of these prefixes:
  - `task/` (recommended) — e.g. `task/123-fix-login` where `123` is the task/issue id
  - `feature/` — e.g. `feature/dark-mode`
- Create and push the branch, and set the upstream: `git checkout -b task/ID-short-desc ; git push -u origin HEAD`.

- Split the assigned task into small sub-tasks. Work on one sub-task at a time. After completing each sub-task:
  1. Build the project from the repo root to verify no breakage: `dotnet build MySchool.csproj`.
  2. Only if the build succeeds, commit the changes with a focused message referencing the task id: `git add . ; git commit -m "task/ID: short description — sub-task"`.
  3. Push the commit to the task branch: `git push`.

- You must commit after EVERY sub-task. Do not bundle multiple sub-tasks into a single commit.

- When the full task is complete, stop and ask the user whether to open a Pull Request to `main` or to continue with additional modifications. Do not open a PR without explicit user approval.

### Pull Request guidelines (when user asks to open one)

- Base branch: `main`. Compare: your task branch -> `main`.
- Title: `task/ID: short title` (include task/issue id if available).
- Description: one short summary followed by a bullet list of changed areas and a short testing checklist. Example:

  Summary: Implements dark mode toggle for main window.

  Changes:
  - `MainWindow.xaml` — added theme toggle button
  - `MainWindow.xaml.cs` — theme apply logic
  - `resources/` — new dark icons

  Testing checklist:
  - Build: `dotnet build MySchool.csproj`
  - Run and verify dark mode toggle via main window UI

## Code Style & Standards
- **Naming**: PascalCase for public members, camelCase for private fields
- **Indentation**: 4 spaces (no tabs)
- **Comments**: XML documentation for public APIs; inline comments only when necessary
- **XAML**: Attributes on separate lines for complex elements
- **License**: All code must be compatible with AGPL v3.0

### Notes and examples from this repo
- Primary UI logic lives in `MainWindow.xaml.cs` — prefer small, incremental edits there and test each change.
- Assets live under `resources/` (e.g., `resources/logo/png/Dark-Icon.png`). Keep added assets in the same subfolder and reference relatively in XAML.
- Dynamic resources are defined in `App.xaml` and updated at runtime by `ThemeManager`.