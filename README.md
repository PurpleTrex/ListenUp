# ListenUp

WPF fallback implementation for a public-domain reader/listener while WinUI 3 templates are unavailable. Once the WinUI 3 templates are installed, this solution can be swapped to a WinUI shell with the same PublicDomain library.

## Projects
- PublicDomain (class library, .NET 8): typed clients for Project Gutenberg, LibriVox, Open Library.
- ListenUp.App (WPF, .NET 8-windows): simple search UI, WebView2 reader window, MediaElement player window.

## Configure APIs
- Gutenberg API host: set AppConfig.GutenbergBaseUrl (defaults to https://project-gutenberg-books-api.p.rapidapi.com).
- Auth headers: set AppConfig.GutenbergApiKeyValue and, if required, host header values. Calls are skipped if the base URL is empty.

## Build/run (WPF fallback)
- Requirements: .NET 8 SDK; WebView2 Runtime (installed with Edge or via https://developer.microsoft.com/microsoft-edge/webview2/); NuGet access to Microsoft.Web.WebView2 (add a reachable feed if nuget.org is blocked).
- Restore/build: `dotnet restore` then `dotnet build` at the solution root.
- Run WPF app: `dotnet run --project ListenUp.App`.

## What to install for the full WinUI 3 app
- Visual Studio 2022 with components:
  - Workloads: .NET Desktop Development, Desktop development with C++.
  - Individual component: Windows App SDK C# Templates (component ID Microsoft.VisualStudio.ComponentGroup.WindowsAppSDK.CS) and Windows App SDK runtime.
- After installation, `dotnet new list winui3` should show templates. Then scaffold the WinUI shell: `dotnet new winui3 -n ListenUp.App.WinUI --framework net8.0` and reference PublicDomain.

## Known gaps / next steps
- EPUB rendering is not implemented (currently opens HTML/PDF via WebView2; EPUB could be converted or handled with a renderer like VersOne.Epub later).
- Audio uses WPF MediaElement; for richer playback consider NAudio or MediaPlayer with background controls.
- No offline caching yet; add a cache under %LocalAppData%/ListenUp for metadata and downloads.
- Minimal error handling; surface more user-friendly toasts and retries (Polly) as needed.
