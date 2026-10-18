# SteamGridDB Metadata Provider NG

A metadata provider plugin for [Playnite](https://playnite.link) that leverages the official [SteamGridDB API v2](https://www.steamgriddb.com/api/v2) to download covers, backgrounds (heroes), logos, and icons for your game library.

---

## ✨ Features

- **Multi-Platform Support**: Matches games automatically by platform ID for:
  - Steam (`steam`)
  - Epic Games Store (`egs`)
  - EA / Origin (`origin`)
  - Battle.net (`bnet`)
  - Ubisoft Connect (`uplay`)
- **Automatic Fallback to Title Search**: If a game's external ID does not have artwork on SteamGridDB, the plugin automatically falls back to searching by the cleaned game title.
- **Artwork Types**:
  - **Covers**: Vertical grids (600x900, 342x482, 660x930), horizontal banners (920x430, 460x215), and square assets (512x512, 1024x1024).
  - **Backgrounds (Heroes)**: High-resolution wallpapers in 1920x620, 3840x1240 (4K), and 1600x650.
  - **Icons & Logos**: Square icons or high-contrast transparent logos.
- **Content Filtering**:
  - **NSFW / Adult content**: Exclude, include only, or allow any.
  - **Humor content**: Exclude, include only, or allow any.
  - **Epilepsy filter**: Exclude flashing/epileptic animations or content.
  - **Image type**: Filter for static images, animated artwork (WebP / APNG), or both.
- **Interactive & Background Scraping**: Select artwork manually using high-res previews or run automated background batch downloads across thousands of games with built-in rate-limit backoff.

---

## 🚀 Installation

1. Download the latest `.pext` package from the [Releases](https://github.com/gOOvER/Playnite-SteamGridDBMetadataNG/releases) page.
2. Drag and drop the downloaded file onto the Playnite window or open Playnite **Settings** -> **Add-ons** -> **Install from file**.
3. Restart Playnite when prompted.

---

## 🔑 Configuration

A SteamGridDB API key is **required** to fetch artwork.

1. Create a free account at [SteamGridDB](https://www.steamgriddb.com).
2. Generate an API Key under your [SteamGridDB API Preferences](https://www.steamgriddb.com/profile/preferences/api).
3. In Playnite, navigate to **Settings** (`F4`) -> **Plugins** -> **SteamGridDB**.
4. Paste your API key into the **SteamGridDB API Key** field.
5. Customize your preferred styles, dimensions, and filter rules, then click **Save**.

---

## 🛠️ Building from Source

### Prerequisites
- [.NET 6.0+ SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or VS Code with C# Dev Kit

### Build Steps
```bash
git clone https://github.com/gOOvER/Playnite-SteamGridDBMetadataNG.git
cd Playnite-SteamGridDBMetadataNG
dotnet restore
dotnet build -c Release
```

The compiled extension `.dll` and assets will be generated in `SGDBMetadata/bin/Release/net462/`.

---

## 📄 License & Credits

- Maintained by **[gOOvER](https://playnite.goover.dev)**
- Originally created by **cooperate**
- Artwork provided via the [SteamGridDB API](https://www.steamgriddb.com/api/v2)
- Licensed under the terms of the original project license.