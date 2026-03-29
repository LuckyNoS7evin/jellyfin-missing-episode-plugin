using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.MissingEpisodes;

/// <summary>
/// Plugin configuration for the Missing Episodes plugin.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets the list of library (folder) IDs to include in the report.
    /// An empty array means all libraries are included.
    /// </summary>
    public string[] IncludedLibraryIds { get; set; } = Array.Empty<string>();
}
