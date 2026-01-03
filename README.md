# Tsundoku

A minimal personal library app for tracking books you’ve bought (your “tsundoku” pile), built with .NET MAUI and C#.

## Features (current)
- Local book library stored in SQLite
- Add/edit books with required fields (title, author, date bought, reason, status)
- Optional fields: ISBN, description, reading dates, review, rating, cover image
- Cover image: pick from photos, pick a file, or take a photo; resized to max 320×480
- ISBN lookup (Open Library) to prefill metadata + cover when available
- Danish + English UI (Danish when system language is Danish, otherwise English)

## Platforms
- macOS (Mac Catalyst)
- iPhone / iPad (iOS)

## Requirements
- .NET SDK: 10.0.x (repo is pinned via `global.json`)
- .NET MAUI workload: `dotnet workload install maui`
- Xcode: 26.x

## Build
Mac Catalyst:

```bash
cd TsundokuApp
dotnet build Tsundoku/Tsundoku.csproj -f net10.0-maccatalyst
```

iOS:

```bash
cd TsundokuApp
dotnet build Tsundoku/Tsundoku.csproj -f net10.0-ios
```

If iOS build fails with an `actool` simulator runtime error, install the matching iOS simulator runtime in Xcode.

## Documents
- Product Requirements Document: [tsundoku_oplæg.md](tsundoku_oplæg.md)

## License
MIT — see [LICENSE](LICENSE).
