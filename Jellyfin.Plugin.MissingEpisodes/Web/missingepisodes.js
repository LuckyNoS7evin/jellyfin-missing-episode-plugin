(function () {
    'use strict';

    var allEpisodes = [];

    function pageInit(page) {
        loadLibraries(page);

        page.querySelector('#refreshBtn').addEventListener('click', function () {
            loadEpisodes(page);
        });

        page.querySelector('#searchInput').addEventListener('input', function () {
            filterTable(page, this.value);
        });

        loadEpisodes(page);
    }

    function loadLibraries(page) {
        ApiClient.fetch({
            type: 'GET',
            url: ApiClient.getUrl('/MissingEpisodes/Libraries'),
            dataType: 'json'
        }).then(function (libraries) {
            var select = page.querySelector('#librarySelect');
            libraries.forEach(function (lib) {
                var option = document.createElement('option');
                option.value = lib.id;
                option.textContent = lib.name;
                select.appendChild(option);
            });
        }).catch(function (err) {
            console.error('Failed to load libraries', err);
        });
    }

    function loadEpisodes(page) {
        var libraryId = page.querySelector('#librarySelect').value;
        var url = ApiClient.getUrl('/MissingEpisodes');
        if (libraryId) {
            url += '?libraryId=' + encodeURIComponent(libraryId);
        }

        page.querySelector('#loadingMsg').style.display = '';
        page.querySelector('#resultsSection').style.display = 'none';
        page.querySelector('#noMissingMsg').style.display = 'none';
        page.querySelector('#summarySection').style.display = 'none';
        page.querySelector('#searchInput').value = '';

        ApiClient.fetch({
            type: 'GET',
            url: url,
            dataType: 'json'
        }).then(function (episodes) {
            allEpisodes = episodes;
            page.querySelector('#loadingMsg').style.display = 'none';

            if (episodes.length === 0) {
                page.querySelector('#noMissingMsg').style.display = '';
            } else {
                page.querySelector('#summaryText').textContent =
                    episodes.length + ' missing episode' + (episodes.length !== 1 ? 's' : '') + ' found.';
                page.querySelector('#summarySection').style.display = '';
                renderTable(page, episodes);
                page.querySelector('#resultsSection').style.display = '';
            }
        }).catch(function (err) {
            page.querySelector('#loadingMsg').style.display = 'none';
            console.error('Failed to load missing episodes', err);
            Dashboard.alert('Failed to load missing episodes. Check the server log for details.');
        });
    }

    function renderTable(page, episodes) {
        var tbody = page.querySelector('#episodeTableBody');
        tbody.innerHTML = '';

        episodes.forEach(function (ep) {
            var tr = document.createElement('tr');

            var premiereText = '';
            if (ep.premiereDate) {
                try {
                    premiereText = new Date(ep.premiereDate).toLocaleDateString();
                } catch (e) {
                    premiereText = ep.premiereDate;
                }
            }

            tr.innerHTML =
                '<td style="padding:6px 8px;">' + escapeHtml(ep.seriesName) + '</td>' +
                '<td style="padding:6px 8px; text-align:center;">' + (ep.seasonNumber != null ? ep.seasonNumber : '?') + '</td>' +
                '<td style="padding:6px 8px; text-align:center;">' + (ep.episodeNumber != null ? ep.episodeNumber : '?') + '</td>' +
                '<td style="padding:6px 8px;">' + escapeHtml(ep.episodeName) + '</td>' +
                '<td style="padding:6px 8px;">' + escapeHtml(premiereText) + '</td>';

            tbody.appendChild(tr);
        });
    }

    function filterTable(page, query) {
        var lower = query.toLowerCase();
        var filtered = allEpisodes.filter(function (ep) {
            return ep.seriesName.toLowerCase().includes(lower) ||
                ep.episodeName.toLowerCase().includes(lower);
        });
        renderTable(page, filtered);
    }

    function escapeHtml(str) {
        if (!str) return '';
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    document.addEventListener('viewshow', function (e) {
        var page = e.target;
        if (page.id === 'missingepisodesPage' || page.dataset.controller === 'missingepisodes') {
            pageInit(page);
        }
    });

})();
