using Newtonsoft.Json;
using Playnite.SDK;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace SGDBMetadata
{
    public class SgdbServiceClient
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private const string BaseUrl = "https://www.steamgriddb.com/api/v2/";

        private readonly RestClient client;
        private SGDBMetadataSettings settings;

        public SgdbServiceClient(SGDBMetadataSettings settings)
        {
            this.settings = settings ?? new SGDBMetadataSettings();
            client = new RestClient(BaseUrl);
            UpdateAuthenticator();
        }

        public void UpdateSettings(SGDBMetadataSettings newSettings)
        {
            settings = newSettings ?? new SGDBMetadataSettings();
            UpdateAuthenticator();
        }

        private void UpdateAuthenticator()
        {
            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                client.Authenticator = new JwtAuthenticator(settings.ApiKey.Trim());
            }
            else
            {
                client.Authenticator = null;
            }
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var fullUrl = client.BuildUri(request);
            logger.Debug($"SGDB Request: {fullUrl}");

            var response = client.Execute(request);

            // Handle HTTP 429 Too Many Requests (Rate Limiting) with one backoff retry
            if (response.StatusCode == (HttpStatusCode)429)
            {
                logger.Warn("SteamGridDB rate limit reached (HTTP 429). Retrying after 1.5 seconds...");
                Thread.Sleep(1500);
                response = client.Execute(request);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.Warn("SteamGridDB API returned 401 Unauthorized. Please verify your API key in plugin settings.");
                return default;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.Debug($"SteamGridDB 404 Not Found for {fullUrl}");
                return default;
            }

            if (response.ErrorException != null)
            {
                logger.Error(response.ErrorException, $"SteamGridDB request error for {fullUrl}: {response.ErrorMessage}");
                return default;
            }

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to deserialize SteamGridDB response for {fullUrl}");
                return default;
            }
        }

        public ResponseModel<SearchModel> getSGDBGames(string searchName)
        {
            if (string.IsNullOrWhiteSpace(searchName))
            {
                return new ResponseModel<SearchModel>();
            }

            logger.Debug($"getSGDBGames: {searchName}");
            var escapedName = Uri.EscapeDataString(searchName.Trim());
            var request = new RestRequest("search/autocomplete/{searchName}", Method.GET);
            request.AddUrlSegment("searchName", escapedName);
            return Execute<ResponseModel<SearchModel>>(request) ?? new ResponseModel<SearchModel>();
        }

        public ResponseModel<GridModel> getSGDBGameGridByAppId(string platform, string gameId)
        {
            var request = new RestRequest("grids/{platform}/{gameId}", Method.GET);
            request.AddUrlSegment("platform", platform);
            request.AddUrlSegment("gameId", gameId);
            ApplyCoverFilters(request);

            return Execute<ResponseModel<GridModel>>(request) ?? new ResponseModel<GridModel>();
        }

        public ResponseModel<GridModel> getSGDBGameGridCover(int gameId)
        {
            var request = new RestRequest("grids/game/{id}", Method.GET);
            request.AddUrlSegment("id", gameId.ToString());
            ApplyCoverFilters(request);

            return Execute<ResponseModel<GridModel>>(request) ?? new ResponseModel<GridModel>();
        }

        private void ApplyCoverFilters(RestRequest request)
        {
            if (settings.CoverStyle != "any")
            {
                request.AddQueryParameter("styles", settings.CoverStyle);
            }
            if (settings.CoverDimension != "any")
            {
                request.AddQueryParameter("dimensions", settings.CoverDimension);
            }
            if (settings.CoverType != "any")
            {
                request.AddQueryParameter("types", settings.CoverType);
            }
            if (settings.CoverNsfw != "any")
            {
                request.AddQueryParameter("nsfw", settings.CoverNsfw);
            }
            if (settings.CoverHumor != "any")
            {
                request.AddQueryParameter("humor", settings.CoverHumor);
            }
            if (settings.CoverEpilepsy != "any")
            {
                request.AddQueryParameter("epilepsy", settings.CoverEpilepsy);
            }
        }

        public ResponseModel<HeroModel> getSGDBGameHero(int gameId)
        {
            // Note: Relative path without leading slash prevents stripping /api/v2/
            var request = new RestRequest("heroes/game/{id}", Method.GET);
            request.AddUrlSegment("id", gameId.ToString());
            ApplyHeroFilters(request);

            return Execute<ResponseModel<HeroModel>>(request) ?? new ResponseModel<HeroModel>();
        }

        public ResponseModel<HeroModel> getSGDBGameHeroByAppId(string platform, string gameId)
        {
            var request = new RestRequest("heroes/{platform}/{gameId}", Method.GET);
            request.AddUrlSegment("platform", platform);
            request.AddUrlSegment("gameId", gameId);
            ApplyHeroFilters(request);

            return Execute<ResponseModel<HeroModel>>(request) ?? new ResponseModel<HeroModel>();
        }

        private void ApplyHeroFilters(RestRequest request)
        {
            if (settings.BackgroundStyle != "any")
            {
                request.AddQueryParameter("styles", settings.BackgroundStyle);
            }
            if (settings.BackgroundDimension != "any")
            {
                request.AddQueryParameter("dimensions", settings.BackgroundDimension);
            }
            if (settings.BackgroundType != "any")
            {
                request.AddQueryParameter("types", settings.BackgroundType);
            }
            if (settings.BackgroundNsfw != "any")
            {
                request.AddQueryParameter("nsfw", settings.BackgroundNsfw);
            }
            if (settings.BackgroundHumor != "any")
            {
                request.AddQueryParameter("humor", settings.BackgroundHumor);
            }
            if (settings.BackgroundEpilepsy != "any")
            {
                request.AddQueryParameter("epilepsy", settings.BackgroundEpilepsy);
            }
        }

        public ResponseModel<MediaModel> getSGDBGameLogo(int gameId)
        {
            var request = new RestRequest("logos/game/{gameId}", Method.GET);
            request.AddUrlSegment("gameId", gameId.ToString());
            ApplyIconOrLogoFilters(request);

            return Execute<ResponseModel<MediaModel>>(request) ?? new ResponseModel<MediaModel>();
        }

        public ResponseModel<MediaModel> getSGDBGameLogoByAppId(string platform, string gameId)
        {
            var request = new RestRequest("logos/{platform}/{gameId}", Method.GET);
            request.AddUrlSegment("platform", platform);
            request.AddUrlSegment("gameId", gameId);
            ApplyIconOrLogoFilters(request);

            return Execute<ResponseModel<MediaModel>>(request) ?? new ResponseModel<MediaModel>();
        }

        public ResponseModel<MediaModel> getSGDBGameIcon(int gameId)
        {
            var request = new RestRequest("icons/game/{gameId}", Method.GET);
            request.AddUrlSegment("gameId", gameId.ToString());
            ApplyIconOrLogoFilters(request);

            return Execute<ResponseModel<MediaModel>>(request) ?? new ResponseModel<MediaModel>();
        }

        public ResponseModel<MediaModel> getSGDBGameIconByAppId(string platform, string gameId)
        {
            var request = new RestRequest("icons/{platform}/{gameId}", Method.GET);
            request.AddUrlSegment("platform", platform);
            request.AddUrlSegment("gameId", gameId);
            ApplyIconOrLogoFilters(request);

            return Execute<ResponseModel<MediaModel>>(request) ?? new ResponseModel<MediaModel>();
        }

        private void ApplyIconOrLogoFilters(RestRequest request)
        {
            if (settings.IconStyle != "any")
            {
                request.AddQueryParameter("styles", settings.IconStyle);
            }
            if (settings.IconType != "any")
            {
                request.AddQueryParameter("types", settings.IconType);
            }
            if (settings.IconNsfw != "any")
            {
                request.AddQueryParameter("nsfw", settings.IconNsfw);
            }
            if (settings.IconHumor != "any")
            {
                request.AddQueryParameter("humor", settings.IconHumor);
            }
            if (settings.IconEpilepsy != "any")
            {
                request.AddQueryParameter("epilepsy", settings.IconEpilepsy);
            }
        }

        public SearchModel getGameSGDBFuzzySearch(string gameTitle)
        {
            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                return null;
            }

            var gameListResponse = getSGDBGames(gameTitle);
            if (gameListResponse?.success == true && gameListResponse.data?.Count > 0)
            {
                return gameListResponse.data[0];
            }

            return null;
        }

        public List<SearchModel> getGameListSGDB(string gameTitle)
        {
            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                return new List<SearchModel>();
            }

            var gameListResponse = getSGDBGames(gameTitle);
            if (gameListResponse?.success == true && gameListResponse.data != null)
            {
                return gameListResponse.data;
            }

            return new List<SearchModel>();
        }

        public string getCoverImageUrl(SearchModel gameSearchItem, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var grid = getSGDBGameGridByAppId(platform, gameId);
                if (grid?.success == true && grid.data?.Count > 0)
                {
                    return grid.data[0].url;
                }
            }

            if (gameSearchItem != null)
            {
                var grid = getSGDBGameGridCover(gameSearchItem.id);
                if (grid?.success == true && grid.data?.Count > 0)
                {
                    return grid.data[0].url;
                }
            }

            return null;
        }

        public List<GridModel> getCoverImages(GenericItemOption searchSelection, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var grid = getSGDBGameGridByAppId(platform, gameId);
                if (grid?.success == true && grid.data?.Count > 0)
                {
                    return grid.data;
                }
            }

            if (searchSelection != null && int.TryParse(searchSelection.Description, out int parsedId))
            {
                var grid = getSGDBGameGridCover(parsedId);
                if (grid?.success == true && grid.data?.Count > 0)
                {
                    return grid.data;
                }
            }

            return null;
        }

        public string getHeroImageUrl(SearchModel gameSearchItem, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var hero = getSGDBGameHeroByAppId(platform, gameId);
                if (hero?.success == true && hero.data?.Count > 0)
                {
                    return hero.data[0].url;
                }
            }

            if (gameSearchItem != null)
            {
                var hero = getSGDBGameHero(gameSearchItem.id);
                if (hero?.success == true && hero.data?.Count > 0)
                {
                    return hero.data[0].url;
                }
            }

            return null;
        }

        public List<HeroModel> getHeroImages(GenericItemOption searchSelection, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var hero = getSGDBGameHeroByAppId(platform, gameId);
                if (hero?.success == true && hero.data?.Count > 0)
                {
                    return hero.data;
                }
            }

            if (searchSelection != null && int.TryParse(searchSelection.Description, out int parsedId))
            {
                var hero = getSGDBGameHero(parsedId);
                if (hero?.success == true && hero.data?.Count > 0)
                {
                    return hero.data;
                }
            }

            return null;
        }

        public string getLogoImageUrl(SearchModel gameSearchItem, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var logo = getSGDBGameLogoByAppId(platform, gameId);
                if (logo?.success == true && logo.data?.Count > 0)
                {
                    return logo.data[0].url;
                }
            }

            if (gameSearchItem != null)
            {
                var logo = getSGDBGameLogo(gameSearchItem.id);
                if (logo?.success == true && logo.data?.Count > 0)
                {
                    return logo.data[0].url;
                }
            }

            return null;
        }

        public List<MediaModel> getLogoImages(GenericItemOption searchSelection, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var logo = getSGDBGameLogoByAppId(platform, gameId);
                if (logo?.success == true && logo.data?.Count > 0)
                {
                    return logo.data;
                }
            }

            if (searchSelection != null && int.TryParse(searchSelection.Description, out int parsedId))
            {
                var logo = getSGDBGameLogo(parsedId);
                if (logo?.success == true && logo.data?.Count > 0)
                {
                    return logo.data;
                }
            }

            return null;
        }

        public string getIconImageUrl(SearchModel gameSearchItem, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var icon = getSGDBGameIconByAppId(platform, gameId);
                if (icon?.success == true && icon.data?.Count > 0)
                {
                    return icon.data[0].url;
                }
            }

            if (gameSearchItem != null)
            {
                var icon = getSGDBGameIcon(gameSearchItem.id);
                if (icon?.success == true && icon.data?.Count > 0)
                {
                    return icon.data[0].url;
                }
            }

            return null;
        }

        public List<MediaModel> getIconImages(GenericItemOption searchSelection, string platform, string gameId)
        {
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(gameId))
            {
                var icon = getSGDBGameIconByAppId(platform, gameId);
                if (icon?.success == true && icon.data?.Count > 0)
                {
                    return icon.data;
                }
            }

            if (searchSelection != null && int.TryParse(searchSelection.Description, out int parsedId))
            {
                var icon = getSGDBGameIcon(parsedId);
                if (icon?.success == true && icon.data?.Count > 0)
                {
                    return icon.data;
                }
            }

            return null;
        }
    }
}
