# Copilot Instructions for MySchool

## Project Overview
MySchool is a WPF application for managing and displaying student and class information. The main UI and logic reside in `MainWindow.xaml` and `MainWindow.xaml.cs`. Application-level configuration is handled in `App.xaml` and `App.xaml.cs`.

## Architecture & Key Files
- `MainWindow.xaml` / `MainWindow.xaml.cs`: Main window UI and code-behind. Entry point for user interaction.
- `Pages/`: Contains additional UI pages (if any) for navigation within the app.
- `Classes/`: Contains C# classes.
- `App.xaml` / `App.xaml.cs`: Application startup, resources, and global event handling.
- `resources/`: Contains assets. Use these for UI branding.
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
- Maintain resource organization when adding new assets.
- Use .NET and WPF conventions for UI and event handling.
- If adding new features, follow the existing code-behind approach unless refactoring to MVVM is explicitly requested.

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

- If the PR includes UI changes, attach 1-2 screenshots to help reviewers.

### Notes and examples from this repo
- Primary UI logic lives in `MainWindow.xaml.cs` — prefer small, incremental edits there and test each change.
- Assets live under `resources/` (e.g., `resources/logo/png/Dark-Icon.png`). Keep added assets in the same subfolder and reference relatively in XAML.