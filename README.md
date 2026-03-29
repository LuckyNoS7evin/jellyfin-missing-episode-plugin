# jellyfin-missing-episode-plugin
A plugin for jellyfin which will list missing episodes and specials from your library

## Features
- **Report page**: A dedicated page in the Jellyfin web UI listing all missing episodes, with search/filter support
- **Dashboard widget**: Shows a quick count of missing episodes with a link to the full report
- **Library filtering**: Scope the report to specific TV libraries or view all at once
- **On-demand**: Refresh the report whenever you like via the Refresh button

## How it works
Jellyfin already stores metadata for all known episodes in a series (sourced from TMDB/TheTVDB), including ones you haven't downloaded yet — these are stored as "virtual" items. This plugin queries those virtual episodes directly from Jellyfin's library, so no external API calls are required.

## Requirements
- Jellyfin 10.11.6+
- .NET 9 SDK (for building)

## Building

```bash
dotnet build
```

The plugin DLL will be output to:
`Jellyfin.Plugin.MissingEpisodes/bin/Debug/net9.0/Jellyfin.Plugin.MissingEpisodes.dll`

## Installation
Copy `Jellyfin.Plugin.MissingEpisodes.dll` to your Jellyfin plugins directory (e.g. `/var/lib/jellyfin/plugins/MissingEpisodes_1.0.0.0/`) and restart Jellyfin.

## API Endpoints
| Method | Path | Description |
|--------|------|-------------|
| GET | `/MissingEpisodes` | List all missing episodes (optional `?libraryId=` filter) |
| GET | `/MissingEpisodes/Count` | Count of missing episodes (optional `?libraryId=` filter) |
| GET | `/MissingEpisodes/Libraries` | List available TV libraries for filtering |

