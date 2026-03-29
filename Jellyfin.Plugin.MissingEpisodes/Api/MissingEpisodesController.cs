using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MissingEpisodes.Api;

/// <summary>
/// Represents a single missing episode in the report.
/// </summary>
public class MissingEpisodeInfo
{
    /// <summary>Gets or sets the series name.</summary>
    public string SeriesName { get; set; } = string.Empty;

    /// <summary>Gets or sets the series Jellyfin ID.</summary>
    public Guid SeriesId { get; set; }

    /// <summary>Gets or sets the season number (null if unknown).</summary>
    public int? SeasonNumber { get; set; }

    /// <summary>Gets or sets the episode number within the season (null if unknown).</summary>
    public int? EpisodeNumber { get; set; }

    /// <summary>Gets or sets the episode name.</summary>
    public string EpisodeName { get; set; } = string.Empty;

    /// <summary>Gets or sets the premiere date of the episode.</summary>
    public DateTime? PremiereDate { get; set; }
}

/// <summary>
/// API controller for the Missing Episodes plugin.
/// </summary>
[ApiController]
[Route("MissingEpisodes")]
[Authorize]
public class MissingEpisodesController : ControllerBase
{
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<MissingEpisodesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingEpisodesController"/> class.
    /// </summary>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger"/> interface.</param>
    public MissingEpisodesController(ILibraryManager libraryManager, ILogger<MissingEpisodesController> logger)
    {
        _libraryManager = libraryManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets missing episodes from the library.
    /// </summary>
    /// <param name="libraryId">Optional library (collection folder) ID to scope the report.</param>
    /// <returns>A list of missing episode information objects.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<MissingEpisodeInfo>> GetMissingEpisodes([FromQuery] Guid? libraryId = null)
    {
        try
        {
            var query = new MediaBrowser.Controller.Entities.InternalItemsQuery
            {
                IncludeItemTypes = new[] { BaseItemKind.Episode },
                IsVirtualItem = true,
                Recursive = true
            };

            if (libraryId.HasValue)
            {
                var library = _libraryManager.GetItemById(libraryId.Value);
                if (library is not null)
                {
                    query.AncestorIds = new[] { libraryId.Value };
                }
            }

            var missingEpisodes = _libraryManager.GetItemList(query)
                .OfType<Episode>()
                .Where(e => e.IsMissingEpisode && !string.IsNullOrEmpty(e.SeriesName))
                .Select(e => new MissingEpisodeInfo
                {
                    SeriesName = e.SeriesName ?? string.Empty,
                    SeriesId = e.SeriesId,
                    SeasonNumber = e.ParentIndexNumber,
                    EpisodeNumber = e.IndexNumber,
                    EpisodeName = e.Name ?? string.Empty,
                    PremiereDate = e.PremiereDate
                })
                .OrderBy(e => e.SeriesName)
                .ThenBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToList();

            return Ok(missingEpisodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MissingEpisodes] Error fetching missing episodes");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Gets a summary count of missing episodes, optionally scoped to a library.
    /// </summary>
    /// <param name="libraryId">Optional library ID to scope the count.</param>
    /// <returns>An object with the total count of missing episodes.</returns>
    [HttpGet("Count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<object> GetMissingEpisodesCount([FromQuery] Guid? libraryId = null)
    {
        try
        {
            var query = new MediaBrowser.Controller.Entities.InternalItemsQuery
            {
                IncludeItemTypes = new[] { BaseItemKind.Episode },
                IsVirtualItem = true,
                Recursive = true
            };

            if (libraryId.HasValue)
            {
                var library = _libraryManager.GetItemById(libraryId.Value);
                if (library is not null)
                {
                    query.AncestorIds = new[] { libraryId.Value };
                }
            }

            var count = _libraryManager.GetItemList(query)
                .OfType<Episode>()
                .Count(e => e.IsMissingEpisode && !string.IsNullOrEmpty(e.SeriesName));

            return Ok(new { Count = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MissingEpisodes] Error fetching episode count");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of TV libraries available for filtering.
    /// </summary>
    /// <returns>A list of library name/ID pairs.</returns>
    [HttpGet("Libraries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetLibraries()
    {
        var libraries = _libraryManager.GetVirtualFolders()
            .Where(f => f.CollectionType.HasValue && f.CollectionType.Value == CollectionTypeOptions.tvshows)
            .Select(f => new
            {
                f.Name,
                Id = f.ItemId
            })
            .ToList();

        return Ok(libraries);
    }
}


