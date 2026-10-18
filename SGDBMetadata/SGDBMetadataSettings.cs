using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SGDBMetadata
{
    public class SGDBMetadataSettings
    {
        public string ApiKey { get; set; } = string.Empty;

        // Covers
        public string CoverStyle { get; set; } = "any";
        public string CoverDimension { get; set; } = "600x900";
        public string CoverNsfw { get; set; } = "false";
        public string CoverHumor { get; set; } = "false";
        public string CoverEpilepsy { get; set; } = "false";
        public string CoverType { get; set; } = "any";

        // Backgrounds (Heroes)
        public string BackgroundStyle { get; set; } = "any";
        public string BackgroundDimension { get; set; } = "any";
        public string BackgroundNsfw { get; set; } = "false";
        public string BackgroundHumor { get; set; } = "false";
        public string BackgroundEpilepsy { get; set; } = "false";
        public string BackgroundType { get; set; } = "any";

        // Icons / Logos
        public string IconAssetSelection { get; set; } = "icons";
        public string IconStyle { get; set; } = "any";
        public string IconNsfw { get; set; } = "false";
        public string IconHumor { get; set; } = "false";
        public string IconEpilepsy { get; set; } = "false";
        public string IconType { get; set; } = "any";

        // Search & Platform Options
        public bool EnablePlatformSearch { get; set; } = true;
        public bool FallbackToTitleSearch { get; set; } = true;
    }

    public class SGDBMetadataSettingsViewModel : ObservableObject, ISettings
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly SGDBMetadata plugin;
        private SGDBMetadataSettings editingClone { get; set; }

        private SGDBMetadataSettings settings;
        public SGDBMetadataSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public SGDBMetadataSettingsViewModel(SGDBMetadata plugin)
        {
            this.plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<SGDBMetadataSettings>();

            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new SGDBMetadataSettings();
            }
        }

        public void BeginEdit()
        {
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            Settings = editingClone;
        }

        public void EndEdit()
        {
            plugin.SavePluginSettings(Settings);
            // Notify service client of updated settings / API key
            plugin.UpdateSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }

        public RelayCommand<object> LoginCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                Login();
            });
        }

        private void Login()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.steamgriddb.com/profile/preferences/api",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to open SteamGridDB API key preferences in default browser.");
            }
        }
    }
}
