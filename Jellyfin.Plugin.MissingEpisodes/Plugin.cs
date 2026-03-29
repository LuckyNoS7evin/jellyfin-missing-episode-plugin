using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.MissingEpisodes;

/// <summary>
/// The Missing Episodes plugin entry point.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "Missing Episodes";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("3e7a8e72-8a85-4b2d-9f3c-1a2b3c4d5e6f");

    /// <inheritdoc />
    public override string Description => "Displays a report of TV episodes missing from your library.";

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = "missingepisodes",
                EmbeddedResourcePath = $"{GetType().Namespace}.Pages.configurationpage.html",
                EnableInMainMenu = true,
                MenuSection = "server",
                MenuIcon = "video_library",
                DisplayName = "Missing Episodes"
            },
            new PluginPageInfo
            {
                Name = "missingepisodes.js",
                EmbeddedResourcePath = $"{GetType().Namespace}.Web.missingepisodes.js"
            },
            new PluginPageInfo
            {
                Name = "dashboard-widget.html",
                EmbeddedResourcePath = $"{GetType().Namespace}.Web.dashboard-widget.html"
            }
        ];
    }
}
