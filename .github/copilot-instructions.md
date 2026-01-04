# Copilot instructions for TsundokuApp

## Project scope (important)
- This is an Apple-only .NET MAUI app: iOS (iPhone/iPad) + Mac Catalyst.
- Do **not** add Android/Windows/Tizen targets, platform folders, or permissions.
- Target frameworks are defined in `Tsundoku/Tsundoku.csproj` (`net10.0-ios;net10.0-maccatalyst`).

## Architecture overview
- MVVM using CommunityToolkit.Mvvm:
  - ViewModels live in `Tsundoku/ViewModels/` and use `[ObservableProperty]` + `[RelayCommand]`.
  - Pages set `BindingContext` in code-behind and load data from `OnAppearing()`.
- Navigation uses Shell routes:
  - Root shell content is defined in `Tsundoku/AppShell.xaml`.
  - Additional routes are registered in `Tsundoku/AppShell.xaml.cs` (e.g., `BookEditPage`).
  - Querystring navigation is used for edit: `BookEditPage` reads `id` via `[QueryProperty]`.
- Persistence is local SQLite:
  - DB init in `Tsundoku/Data/TsundokuDb.cs`, file stored under `FileSystem.AppDataDirectory/tsundoku.db3`.
  - CRUD via `Tsundoku/Services/IBookRepository.cs` + `BookRepository`.
  - The entity is `Tsundoku/Models/Book.cs` (sqlite-net attributes).
- Book metadata lookup:
  - `Tsundoku/Services/OpenLibraryBookLookupService.cs` does ISBN lookup via Open Library `search.json?isbn=...`.
  - Network services use DI-provided `HttpClient` (`AddHttpClient` in `Tsundoku/MauiProgram.cs`).
- Images:
  - `Tsundoku/Services/ImageService.cs` stores JPEGs under `FileSystem.AppDataDirectory/images/`.
  - Always resize to fit within 320×480 (preserve aspect ratio); DB stores only the file path.

## Localization conventions (DA/EN)
- Strings are RESX-backed:
  - English: `Tsundoku/Resources/Strings/AppResources.resx`
  - Danish: `Tsundoku/Resources/Strings/AppResources.da.resx`
- XAML uses the custom markup extension: `{markup:Translate Key=SomeKey}` from `Tsundoku/MarkupExtensions/TranslateExtension.cs`.
- Culture selection rule is enforced at startup in `Tsundoku/Localization/LocalizationConfigurator.cs`:
  - If `CurrentUICulture.TwoLetterISOLanguageName == "da"` use `da-DK`, else `en-US`.
- If you add UI text, add keys to both RESX files (and keep keys stable).

## Adding a new page/flow (project pattern)
- Create the page in `Tsundoku/Pages/` and a viewmodel in `Tsundoku/ViewModels/`.
- Register both in DI in `Tsundoku/MauiProgram.cs` (viewmodels + pages are `AddTransient`).
- If it’s navigated to via Shell route, register the route in `Tsundoku/AppShell.xaml.cs`.

## Developer workflows
- SDK is pinned by `global.json` (use `dotnet --info` if builds behave oddly).
- Build commands (see `README.md`):
  - Mac Catalyst: `dotnet build Tsundoku/Tsundoku.csproj -f net10.0-maccatalyst`
  - iOS: `dotnet build Tsundoku/Tsundoku.csproj -f net10.0-ios`
- If iOS build fails with an `actool` simulator runtime error, install the matching iOS simulator runtime in Xcode.
