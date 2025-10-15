# Copilot Instructions for MySchool

## Project Overview
MySchool is a WPF application for managing and displaying student and class information. The main UI and logic reside in `MainWindow.xaml` and `MainWindow.xaml.cs`. Application-level configuration is handled in `App.xaml` and `App.xaml.cs`.

## Architecture & Key Files
- `MainWindow.xaml` / `MainWindow.xaml.cs`: Main window UI and code-behind. Entry point for user interaction.
- `App.xaml` / `App.xaml.cs`: Application startup, resources, and global event handling.
- `resources/`: Contains fonts and logo assets. Use these for UI branding.
- `MySchool.csproj`: Project configuration, target framework (net9.0-windows), and dependencies.

## Developer Workflows
- **Build:** Use Visual Studio or run `dotnet build MySchool.csproj` from the project root.
- **Run:** Launch via Visual Studio or `dotnet run --project MySchool.csproj`.
- **Debug:** Preferred in Visual Studio for WPF UI debugging.
- **Assets:** Reference images and fonts from the `resources/` directory using relative paths in XAML.

## Patterns & Conventions
- Code-behind pattern is used for UI logic (no MVVM or data binding frameworks detected).
- UI resources (images, fonts) are organized under `resources/` with subfolders for format and transparency.
- Project targets Windows only (`net9.0-windows`).
- No custom build scripts, tests, or external service integrations detected.

## Examples
- To display a logo in XAML:
  ```xml
  <Image Source="resources/logo/png/Dark-Icon.png" Width="125" />
  ```
- To handle application startup logic, use `App.xaml.cs`.

## Recommendations for AI Agents
- Focus on code-behind logic in `MainWindow.xaml.cs` for UI features.
- Maintain resource organization when adding new assets.
- Use .NET and WPF conventions for UI and event handling.
- If adding new features, follow the existing code-behind approach unless refactoring to MVVM is explicitly requested.

---
If any section is unclear or missing, please provide feedback for further refinement.
