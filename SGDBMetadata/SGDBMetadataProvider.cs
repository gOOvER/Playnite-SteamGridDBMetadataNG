using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGDBMetadata
{
    public class SGDBMetadataProvider : OnDemandMetadataProvider
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly MetadataRequestOptions options;
        private readonly SGDBMetadata plugin;
        private readonly SGDBMetadataSettings settings;
        private readonly SgdbServiceClient services;

        private GenericItemOption searchSelection;
        private SearchModel gameSearchItem;
        private string gamePlatformEnum;
        private List<MetadataField> availableFields;
        private bool hasPromptedGameSelection = false;

        private const string demoSuffix = " Demo";

        public override List<MetadataField> AvailableFields
        {
            get
            {
                if (availableFields == null)
                {
                    availableFields = GetAvailableFields();
                }
                return availableFields;
            }
        }

        public SGDBMetadataProvider(MetadataRequestOptions options, SGDBMetadata plugin, SGDBMetadataSettings settings, SgdbServiceClient client)
        {
            this.options = options;
            this.plugin = plugin;
            this.settings = settings;
            this.services = client ?? new SgdbServiceClient(settings);
        }

        public void ConvertPlayniteGamePluginIdToSGDBPlatformEnum(Guid pluginId, string gameId)
        {
            if (!settings.EnablePlatformSearch)
            {
                return;
            }

            switch (BuiltinExtensions.GetExtensionFromId(pluginId))
            {
                case BuiltinExtension.SteamLibrary:
                    // Normal Steam AppIds are within int32 range; mods or demo entries fallback to title search
                    if (int.TryParse(gameId, out _) &&
                        !options.GameData.Name.EndsWith(demoSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        gamePlatformEnum = "steam";
                    }
                    return;
                case BuiltinExtension.EpicLibrary:
                    gamePlatformEnum = "egs";
                    return;
                case BuiltinExtension.OriginLibrary:
                    gamePlatformEnum = "origin";
                    return;
                case BuiltinExtension.BattleNetLibrary:
                    gamePlatformEnum = "bnet";
                    return;
                case BuiltinExtension.UplayLibrary:
                    gamePlatformEnum = "uplay";
                    return;
                default:
                    return;
            }
        }

        private List<MetadataField> GetAvailableFields()
        {
            if (searchSelection == null && gamePlatformEnum == null)
            {
                GetSgdbMetadata();
            }

            return new List<MetadataField>
            {
                MetadataField.CoverImage,
                MetadataField.BackgroundImage,
                MetadataField.Icon
            };
        }

        private void GetSgdbMetadata()
        {
            ConvertPlayniteGamePluginIdToSGDBPlatformEnum(options.GameData.PluginId, options.GameData.GameId);
            var cleanedUpGameName = CleanupGameName(options.GameData.Name);

            if (!options.IsBackgroundDownload)
            {
                if (string.IsNullOrEmpty(gamePlatformEnum))
                {
                    PromptChooseGameIfNeeded();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(gamePlatformEnum))
                {
                    gameSearchItem = services.getGameSGDBFuzzySearch(cleanedUpGameName);
                }
            }
        }

        private void PromptChooseGameIfNeeded()
        {
            if (hasPromptedGameSelection || searchSelection != null)
            {
                return;
            }

            hasPromptedGameSelection = true;
            var cleanedUpGameName = CleanupGameName(options.GameData.Name);

            var initialList = services.getGameListSGDB(cleanedUpGameName)
                .Select(game => new GenericItemOption(game.name, game.id.ToString()))
                .ToList();

            var item = plugin.PlayniteApi.Dialogs.ChooseItemWithSearch(initialList, (query) =>
            {
                try
                {
                    return services.getGameListSGDB(query)
                        .Select(game => new GenericItemOption(game.name, game.id.ToString()))
                        .ToList();
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, $"Failed to search SGDB games for query: {query}");
                    return new List<GenericItemOption>();
                }
            }, cleanedUpGameName, ResourceProvider.GetString("LOCSteamGridDBMetadata_WindowTitleChooseGame"));

            searchSelection = item;
        }

        private string CleanupGameName(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                return string.Empty;
            }

            if (gameName.EndsWith(demoSuffix, StringComparison.OrdinalIgnoreCase))
            {
                gameName = gameName.Substring(0, gameName.Length - demoSuffix.Length);
            }

            return gameName.Trim();
        }

        public override MetadataFile GetCoverImage(GetMetadataFieldArgs args)
        {
            if (options.IsBackgroundDownload)
            {
                // 1. Try platform lookup if available
                string gameUrl = services.getCoverImageUrl(gameSearchItem, gamePlatformEnum, options.GameData.GameId);

                // 2. Fallback to game title search if platform search gave no results
                if (string.IsNullOrEmpty(gameUrl) && settings.FallbackToTitleSearch)
                {
                    if (gameSearchItem == null)
                    {
                        gameSearchItem = services.getGameSGDBFuzzySearch(CleanupGameName(options.GameData.Name));
                    }

                    if (gameSearchItem != null)
                    {
                        gameUrl = services.getCoverImageUrl(gameSearchItem, null, null);
                    }
                }

                if (!string.IsNullOrEmpty(gameUrl))
                {
                    return new MetadataFile(gameUrl);
                }
            }
            else
            {
                // Manual selection mode
                var covers = services.getCoverImages(searchSelection, gamePlatformEnum, options.GameData.GameId);

                // Fallback: If platform ID returned nothing, prompt user to search by title
                if ((covers == null || covers.Count == 0) && settings.FallbackToTitleSearch && searchSelection == null)
                {
                    PromptChooseGameIfNeeded();
                    if (searchSelection != null)
                    {
                        covers = services.getCoverImages(searchSelection, null, null);
                    }
                }

                if (covers != null && covers.Count > 0)
                {
                    var selection = GetCoverManually(covers) as ThumbFileOption;
                    if (selection != null && !string.IsNullOrEmpty(selection.FullRes))
                    {
                        return new MetadataFile(selection.FullRes);
                    }
                }
            }

            return base.GetCoverImage(args);
        }

        public override MetadataFile GetBackgroundImage(GetMetadataFieldArgs args)
        {
            if (options.IsBackgroundDownload)
            {
                string gameUrl = services.getHeroImageUrl(gameSearchItem, gamePlatformEnum, options.GameData.GameId);

                if (string.IsNullOrEmpty(gameUrl) && settings.FallbackToTitleSearch)
                {
                    if (gameSearchItem == null)
                    {
                        gameSearchItem = services.getGameSGDBFuzzySearch(CleanupGameName(options.GameData.Name));
                    }

                    if (gameSearchItem != null)
                    {
                        gameUrl = services.getHeroImageUrl(gameSearchItem, null, null);
                    }
                }

                if (!string.IsNullOrEmpty(gameUrl))
                {
                    return new MetadataFile(gameUrl);
                }
            }
            else
            {
                var heroes = services.getHeroImages(searchSelection, gamePlatformEnum, options.GameData.GameId);

                if ((heroes == null || heroes.Count == 0) && settings.FallbackToTitleSearch && searchSelection == null)
                {
                    PromptChooseGameIfNeeded();
                    if (searchSelection != null)
                    {
                        heroes = services.getHeroImages(searchSelection, null, null);
                    }
                }

                if (heroes != null && heroes.Count > 0)
                {
                    var selection = GetHeroManually(heroes) as ThumbFileOption;
                    if (selection != null && !string.IsNullOrEmpty(selection.FullRes))
                    {
                        return new MetadataFile(selection.FullRes);
                    }
                }
            }

            return base.GetBackgroundImage(args);
        }

        public override MetadataFile GetIcon(GetMetadataFieldArgs args)
        {
            bool useLogos = settings.IconAssetSelection == "logos";

            if (options.IsBackgroundDownload)
            {
                string gameUrl = useLogos
                    ? services.getLogoImageUrl(gameSearchItem, gamePlatformEnum, options.GameData.GameId)
                    : services.getIconImageUrl(gameSearchItem, gamePlatformEnum, options.GameData.GameId);

                if (string.IsNullOrEmpty(gameUrl) && settings.FallbackToTitleSearch)
                {
                    if (gameSearchItem == null)
                    {
                        gameSearchItem = services.getGameSGDBFuzzySearch(CleanupGameName(options.GameData.Name));
                    }

                    if (gameSearchItem != null)
                    {
                        gameUrl = useLogos
                            ? services.getLogoImageUrl(gameSearchItem, null, null)
                            : services.getIconImageUrl(gameSearchItem, null, null);
                    }
                }

                if (!string.IsNullOrEmpty(gameUrl))
                {
                    return new MetadataFile(gameUrl);
                }
            }
            else
            {
                var icons = useLogos
                    ? services.getLogoImages(searchSelection, gamePlatformEnum, options.GameData.GameId)
                    : services.getIconImages(searchSelection, gamePlatformEnum, options.GameData.GameId);

                if ((icons == null || icons.Count == 0) && settings.FallbackToTitleSearch && searchSelection == null)
                {
                    PromptChooseGameIfNeeded();
                    if (searchSelection != null)
                    {
                        icons = useLogos
                            ? services.getLogoImages(searchSelection, null, null)
                            : services.getIconImages(searchSelection, null, null);
                    }
                }

                if (icons != null && icons.Count > 0)
                {
                    var selection = GetIconManually(icons) as ThumbFileOption;
                    if (selection != null && !string.IsNullOrEmpty(selection.FullRes))
                    {
                        return new MetadataFile(selection.FullRes);
                    }
                }
            }

            return base.GetIcon(args);
        }

        private ImageFileOption GetCoverManually(List<GridModel> possibleCovers)
        {
            if (possibleCovers == null || possibleCovers.Count == 0)
            {
                return null;
            }

            var selection = possibleCovers.Select(cover => new ThumbFileOption
            {
                Path = cover.thumb,
                FullRes = cover.url
            }).Cast<ImageFileOption>().ToList();

            return plugin.PlayniteApi.Dialogs.ChooseImageFile(
                selection, ResourceProvider.GetString("LOCSteamGridDBMetadata_WindowTitleChooseCover"));
        }

        private ImageFileOption GetHeroManually(List<HeroModel> possibleHeroes)
        {
            if (possibleHeroes == null || possibleHeroes.Count == 0)
            {
                return null;
            }

            var selection = possibleHeroes.Select(hero => new ThumbFileOption
            {
                Path = hero.thumb,
                FullRes = hero.url
            }).Cast<ImageFileOption>().ToList();

            return plugin.PlayniteApi.Dialogs.ChooseImageFile(
                selection, ResourceProvider.GetString("LOCSteamGridDBMetadata_WindowTitleChooseBackground"));
        }

        private ImageFileOption GetIconManually(List<MediaModel> possibleIcons)
        {
            if (possibleIcons == null || possibleIcons.Count == 0)
            {
                return null;
            }

            var selection = possibleIcons.Select(icon => new ThumbFileOption
            {
                Path = icon.thumb,
                FullRes = icon.url
            }).Cast<ImageFileOption>().ToList();

            return plugin.PlayniteApi.Dialogs.ChooseImageFile(
                selection, ResourceProvider.GetString("LOCSteamGridDBMetadata_WindowTitleChooseIcon"));
        }
    }
}
