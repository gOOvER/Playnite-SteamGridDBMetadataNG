# Changelog

All notable changes to **SteamGridDB Metadata Provider NG** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-10-18

### Added
- **Multi-Platform Support**: Re-enabled native SteamGridDB platform lookup for Epic Games Store (`egs`), EA / Origin (`origin`), Battle.net (`bnet`), and Ubisoft (`uplay`) alongside Steam.
- **Automatic Title Fallback**: If a game's platform ID search yields 0 artwork results (e.g. fresh releases, unlinked IDs, or demo entries), the provider now automatically falls back to searching by game name.
- **Epilepsy Filtering**: Added `epilepsy` query parameter filtering (`exclude`, `only`, `any`) across Covers, Backgrounds, and Icons to protect photosensitive gamers.
- **Image Type Filter**: Added option to filter for `static` images only, `animated` images only (WebP / APNG), or `any`.
- **Icon & Logo Styles**: Added style filtering for Icons (`official`, `custom`) and Logos (`official`, `white`, `black`, `custom`).
- **Rate Limit Resilience**: Added automatic HTTP 429 backoff handling with retry when SteamGridDB rate limits are encountered during batch downloads.
- **API Key Warning**: Added clear UI warning and browser launch fallback when the API key is not configured.
- **Project Modernization**: Migrated to SDK-style `.csproj` targeting .NET Framework 4.6.2 and WPF; updated `PlayniteSDK` to 6.18.0 and `RestSharp` to 106.15.0.

### Changed
- **Rebrand to NG**: Rebranded extension to **SteamGridDB Metadata Provider NG** maintained by `gOOvER`, retaining extension ID `SteamGridDB_Playnite_Metadata` for backwards compatibility with existing user settings.
- **Settings UX**: Overhauled settings interface with organized sections for General/Matching, Cover, Background, and Icon/Logo configurations.
- **Client Lifetime**: Reused `RestClient` instance across game downloads rather than instantiating a new client per game, preventing socket exhaustion.
- **Assembly Metadata**: Updated `AssemblyInfo.cs` versioning to 1.0.0.0.

### Fixed
- **Hero Image Leading Slash Bug**: Fixed leading slash in `/heroes/game/{id}` causing RestSharp to strip the `/api/v2/` base path and 404 on background lookups.
- **Search Autocomplete Crash (`IndexOutOfRangeException`)**: Fixed unhandled exception when searching for a game title that returns 0 results from SteamGridDB.
- **Manual Image Selection Crash (`RuntimeBinderException`)**: Removed unsafe `dynamic` typing and fixed boolean `||` logic in manual image dialog handling.
- **Path Traversal / URL Injection in Search**: Ensured game titles with special characters (`/`, `\`, `?`, `#`, `&`) are properly URL-escaped before making API autocomplete requests.
- **Release Date Model Typo**: Fixed `release_data` typo in `SearchModel` to properly map `release_date` as a 64-bit Unix timestamp.
- **Graceful Error Handling**: Replaced hard exceptions on HTTP 401, 404, or network errors with structured warnings and graceful fallbacks, preventing Playnite metadata download crashes.
- **Safe Browser Launch**: Configured `ProcessStartInfo.UseShellExecute = true` when opening the SteamGridDB API key preferences page to prevent `Win32Exception`.

### Removed
- **Dead Code**: Removed unreferenced and leaky `ImageConversion.cs` and `ImageDownloader.cs`.
- **Legacy MSBuild Artifacts**: Removed `packages.config` in favor of modern SDK `<PackageReference>`.
- **Log Flooding**: Removed excessive `logger.Info` calls logging full API request URLs and complete JSON payloads to `playnite.log`.

---

## [Legacy] - Upstream cooperative / cooperate releases
- **Version 1.2** (2024-10-02): Fixed Steam demo metadata lookup and demo suffix handling.
- **Version 1.1** (2021-07-05): Added dimensions options, background style fix, mod AppId handling.
- **Version 1.0** (2021-09-08): Initial Playnite 9 release with localization support.
