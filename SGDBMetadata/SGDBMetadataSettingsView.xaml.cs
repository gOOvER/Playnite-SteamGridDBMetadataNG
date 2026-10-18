using Playnite.SDK;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SGDBMetadata
{
    public partial class SGDBMetadataSettingsView : UserControl
    {
        public SGDBMetadataSettingsView()
        {
            InitializeComponent();

            // Generic filters (Adult/NSFW, Humor, Epilepsy)
            var cmbGenericNsfw = new Dictionary<string, string>
            {
                { "false", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericNotAdult") },
                { "true", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericOnlyAdult") },
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") }
            };

            var cmbGenericHumor = new Dictionary<string, string>
            {
                { "false", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericNotHumor") },
                { "true", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericOnlyHumor") },
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") }
            };

            var cmbGenericEpilepsy = new Dictionary<string, string>
            {
                { "false", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericNotEpilepsy") },
                { "true", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericOnlyEpilepsy") },
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") }
            };

            var cmbGenericTypes = new Dictionary<string, string>
            {
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "static", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingTypeStatic") },
                { "animated", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingTypeAnimated") }
            };

            // Cover items
            cmbCoverStyles.ItemsSource = new Dictionary<string, string>
            {
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "alternate", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleAlternate") },
                { "blurred", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleBlurred") },
                { "white_logo", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleWhiteLogo") },
                { "material", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleMaterial") },
                { "no_logo", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleNoLogo") }
            };

            cmbCoverDimensions.ItemsSource = new Dictionary<string, string>
            {
                { "600x900", "600x900 (Vertical)" },
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "460x215", "460x215 (Legacy)" },
                { "920x430", "920x430 (Horizontal)" },
                { "460x215,920x430", "460x215 & 920x430" },
                { "342x482", "342x482" },
                { "660x930", "660x930" },
                { "512x512", "512x512 (Square)" },
                { "1024x1024", "1024x1024 (Square HD)" },
                { "512x512,1024x1024", "512x512 & 1024x1024" }
            };

            cmbCoverType.ItemsSource = cmbGenericTypes;
            cmbCoverNsfw.ItemsSource = cmbGenericNsfw;
            cmbCoverHumor.ItemsSource = cmbGenericHumor;
            cmbCoverEpilepsy.ItemsSource = cmbGenericEpilepsy;

            // Background (Hero) items
            cmbBackgroundStyles.ItemsSource = new Dictionary<string, string>
            {
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "alternate", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleAlternate") },
                { "blurred", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleBlurred") },
                { "material", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleMaterial") }
            };

            cmbBackgroundDimensions.ItemsSource = new Dictionary<string, string>
            {
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "1920x620", "1920x620" },
                { "3840x1240", "3840x1240 (4K)" },
                { "1600x650", "1600x650" }
            };

            cmbBackgroundType.ItemsSource = cmbGenericTypes;
            cmbBackgroundNsfw.ItemsSource = cmbGenericNsfw;
            cmbBackgroundHumor.ItemsSource = cmbGenericHumor;
            cmbBackgroundEpilepsy.ItemsSource = cmbGenericEpilepsy;

            // Icon / Logo items
            cmbIconAssetSelection.ItemsSource = new Dictionary<string, string>
            {
                { "icons", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingIconAssetIcon") },
                { "logos", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingIconAssetLogo") }
            };

            cmbIconStyles.ItemsSource = new Dictionary<string, string>
            {
                { "any", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingGenericAny") },
                { "official", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleOfficial") },
                { "custom", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleCustom") },
                { "white", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleWhite") },
                { "black", ResourceProvider.GetString("LOCSteamGridDBMetadata_SettingStyleBlack") }
            };

            cmbIconType.ItemsSource = cmbGenericTypes;
            cmbIconNsfw.ItemsSource = cmbGenericNsfw;
            cmbIconHumor.ItemsSource = cmbGenericHumor;
            cmbIconEpilepsy.ItemsSource = cmbGenericEpilepsy;
        }
    }
}
