# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build (Windows target for dev iteration)
dotnet build DuoMusica/DuoMusica.csproj -f net10.0-windows10.0.19041.0

# Build for Android
dotnet build DuoMusica/DuoMusica.csproj -f net10.0-android

# Build for iOS (requires Mac)
dotnet build DuoMusica/DuoMusica.csproj -f net10.0-ios

# Restore packages
dotnet restore DuoMusica/DuoMusica.csproj

# Run on Windows (dev/debug only)
dotnet run DuoMusica/DuoMusica.csproj -f net10.0-windows10.0.19041.0
```

No test project exists yet. When adding one: `dotnet test`.

## API Keys

Set both keys in `DuoMusica/AppSettings.cs` before running:

- `AppSettings.SpotifyClientId` — from [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
- `AppSettings.MusixmatchApiKey` — from [Musixmatch Developer Portal](https://developer.musixmatch.com)

Also register `duomusica://callback` as a redirect URI in the Spotify dashboard.

## Architecture

**Stack**: .NET 10 MAUI, CommunityToolkit.Mvvm, CommunityToolkit.Maui, sqlite-net-pcl.

**MVVM pattern**: all ViewModels in `DuoMusica/ViewModels/` extend `BaseViewModel` and use `[ObservableProperty]` / `[RelayCommand]` source generators from CommunityToolkit.Mvvm. Views wire up via constructor injection (DI in `MauiProgram.cs`).

**Navigation**: Shell-based, all routes registered in `AppShell.xaml.cs`. Use `Shell.Current.GoToAsync(nameof(SomePage), parameters)` with `[QueryProperty]` on the target ViewModel.

**Key flows**:

1. First launch → `OnboardingPage` (language/level setup) → saved to SQLite via `UserRepository`
2. `HomePage` → mode selection → `SongSearchPage` (Spotify search + genre filter)
3. Song selected → `GamePage` → `GameService.BuildSessionAsync()` fetches Musixmatch lyrics, parses LRC via `LrcParser`, splits into 15-20s `SongSegment`s via `SegmentSplitter`
4. Each segment: `ISpotifyRemoteService.PlaySegmentAsync()` → user types → `TextDiffHelper.Compare()` → `ResultsPage` with colored diff
5. All segments done → `SongCompletePage` with XP award via `XpCalculator`
6. Deathmatch: same flow but `DeathMatchViewModel` runs `BotService` on a 200ms timer alongside user typing

**Services DI registration** (`MauiProgram.cs`):
- Singletons: `AppDatabase`, repositories, Spotify services, `ILyricsService`, `IUserProfileService`, `IDifficultyService`, `IGameService`
- Transient: `IBotService`, all ViewModels and Pages
- `ISpotifyRemoteService` resolves to platform-specific impl (`#if ANDROID` / `#elif IOS`) or `SpotifyRemoteServiceStub` on Windows

**Spotify integration**:
- Auth: OAuth 2.0 PKCE via `SpotifyAuthService` using MAUI `WebAuthenticator`. Tokens stored in `SecureStorage`.
- Search/metadata: `SpotifyWebApiService` (REST, works on all platforms)
- Playback control: `ISpotifyRemoteService` — **stubs only** in `Platforms/Android/Services/` and `Platforms/iOS/Services/`. To make playback work, add native SDK bindings (see TODO comments in each stub file).

**Difficulty scoring** (`DifficultyService`): `score = wordCount×0.3 + avgWordLength×5×0.2 + uniqueWordRatio×100×0.3 + (bpm/200)×100×0.2`. Maps to `Beginner`/<30, `Intermediate`/30-60, `Advanced`/>60.

**Text diff** (`TextDiffHelper`): normalizes via `StringNormalizer` (lowercase, strip accents/punctuation), word-level comparison using Levenshtein ≤1 = "Close" (0.5 pts), exact = "Correct" (1.0 pt). Score = `earnedPoints / totalWords × 100`.

**XP** (`XpCalculator`): `base = 100 * (score/100)²`, ×1.5 first attempt, ×2.0 perfect. Level thresholds: 0, 200, 500, 1000, 2000, 3500, 5500, 8000, 11000, 15000.

**Database** (`AppDatabase`, `sqlite-net-pcl`): tables `UserProfile`, `SongProgress`, `SegmentProgress`. DB file at `FileSystem.AppDataDirectory/duomusica.db3`.

**Custom value converters** (registered globally in `App.xaml`):
- `InvertBool`, `IsNotNullOrEmpty`, `IntEquals`, `IsPositive`, `BoolToString` — all in `Helpers/Converters.cs`
