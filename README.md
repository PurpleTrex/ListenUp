# ListenUp

A full-featured WPF application for reading public domain books and listening to audiobooks. This solution includes a .NET 10 class library for accessing Project Gutenberg, LibriVox, and Open Library APIs, plus a modern WPF UI with comprehensive features.

## Projects
- **PublicDomain** (class library, .NET 10): Typed clients for Project Gutenberg, LibriVox, and Open Library APIs
- **ListenUp.App** (WPF, .NET 10-windows): Feature-rich UI with reader, player, favorites, caching, and settings

## Features

### 📚 Book Search & Discovery
- Unified search across Project Gutenberg, LibriVox, and Open Library
- Automatic result aggregation and deduplication by title/author
- Search result caching (24-hour default) for faster repeat searches
- Cover art display from Open Library
- Visual indicators for available formats (text/audio)

### 📖 Reading
- **EPUB Reader**: Full EPUB support with chapter navigation, table of contents
- **HTML/PDF Viewer**: WebView2-based reader for HTML and PDF files
- Clean reading interface with chapter controls
- Automatic format detection and selection

### 🎧 Audio Player
- Full playback controls: play, pause, stop
- Seek controls: +/- 10 seconds
- Progress slider with time display
- Volume control
- Display current/total time
- Auto-play on load

### ⭐ Favorites & Bookmarks
- Add/remove favorites with one click
- SQLite-backed persistent storage
- View all favorites from the menu
- Quick access to frequently accessed content

### 💾 Offline Caching
- SQLite database for metadata caching
- Automatic cache management
- Configurable cache duration
- Manual cache clearing option

### 🔄 Resilience & Error Handling
- Retry logic with exponential backoff (Polly)
- Configurable timeout (30-45s default)
- Graceful error messages
- Connection failure handling

### ⚙️ Settings
- Configure Gutenberg API keys and endpoints
- Cache management
- API configuration (URL, headers, keys)
- Settings persistence

## Technology Stack
- **.NET 10** (latest)
- **WPF** for UI
- **WebView2** for HTML/PDF rendering
- **VersOne.Epub 3.3.5** for EPUB reading
- **Polly 8.6.5** for resilience policies
- **Microsoft.Data.Sqlite 10.0.2** for caching
- **System.Text.Json** for serialization

## Configure APIs
- **Gutenberg API**: Set in Settings window or AppConfig.cs
  - Base URL (defaults to https://project-gutenberg-books-api.p.rapidapi.com)
  - API Key name and value
  - Host header
- **LibriVox**: No configuration required (public API)
- **Open Library**: No configuration required (public API)

## Build/Run

### Requirements
- .NET 10 SDK (included in Visual Studio 2022 or downloadable)
- WebView2 Runtime (pre-installed with Microsoft Edge or download from https://developer.microsoft.com/microsoft-edge/webview2/)
- Windows OS (WPF requirement)

### Build
```bash
dotnet restore
dotnet build
```

### Run
```bash
dotnet run --project ListenUp.App
```

Or open `ListenUp.sln` in Visual Studio 2022 and press F5.

## Keyboard Shortcuts
- **Enter** in search box: Execute search
- **Esc** in EPUB reader TOC: Close table of contents

## Data Storage
- **Cache Database**: `%LocalAppData%/ListenUp/cache.db`
- **Favorites**: Stored in the same SQLite database
- **Temp EPUB files**: Downloaded to system temp folder

## Future WinUI 3 Migration
This is a WPF implementation. To migrate to WinUI 3:
1. Install Visual Studio 2022 with:
   - Workload: .NET Desktop Development
   - Workload: Desktop development with C++
   - Component: Windows App SDK C# Templates (Microsoft.VisualStudio.ComponentGroup.WindowsAppSDK.CS)
2. Verify templates: `dotnet new list winui3`
3. Create WinUI shell: `dotnet new winui3 -n ListenUp.App.WinUI --framework net10.0`
4. Reference the PublicDomain library
5. Migrate UI components and update APIs for WinUI 3

## Architecture

### Services
- **SearchService**: Aggregates searches across all APIs
- **CacheService**: SQLite-backed caching and favorites
- **ResilienceService**: Retry policies and circuit breakers

### View Models
- **MainViewModel**: MVVM pattern with INotifyPropertyChanged
- Observable collections for reactive UI updates

### Windows
- **MainWindow**: Main app with search and results
- **ReaderWindow**: WebView2 HTML/PDF reader
- **EpubReaderWindow**: EPUB reader with navigation
- **PlayerWindow**: Audio player with full controls
- **SettingsWindow**: Configuration management

## Dependencies (NuGet)
```xml
<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.3719.77" />
<PackageReference Include="VersOne.Epub" Version="3.3.5" />
<PackageReference Include="Polly" Version="8.6.5" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.2" />
```

## Known Limitations
- WPF is Windows-only (cannot run on Linux/Mac)
- EPUB rendering strips HTML tags (basic text display)
- No background audio playback
- LibriVox audio URLs may have CORS limitations
- Cache is not encrypted

## Contributing
This is a demonstration project. For production use, consider:
- Adding unit tests
- Implementing user authentication
- Adding sync across devices
- Enhanced EPUB rendering with CSS support
- Background audio service
- Download management for offline reading
- Content recommendations

## License
This project is for educational purposes. Remember to comply with API terms of service for Project Gutenberg, LibriVox, and Open Library when using this application.
