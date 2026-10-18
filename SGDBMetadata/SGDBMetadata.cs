using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SGDBMetadata
{
    public class SGDBMetadata : MetadataPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private SGDBMetadataSettingsViewModel settings { get; set; }

        public SgdbServiceClient Client { get; private set; }

        public override Guid Id { get; } = Guid.Parse("f9a763e1-1ccb-4d7d-b955-d59e708f71c1");

        public override List<MetadataField> SupportedFields { get; } = new List<MetadataField>
        {
            MetadataField.CoverImage,
            MetadataField.BackgroundImage,
            MetadataField.Icon,
            MetadataField.Links
        };

        public override string Name => "SteamGridDB";

        public SGDBMetadata(IPlayniteAPI api) : base(api)
        {
            settings = new SGDBMetadataSettingsViewModel(this);
            Client = new SgdbServiceClient(settings.Settings);
            Properties = new MetadataPluginProperties
            {
                HasSettings = true
            };
        }

        public void UpdateSettings(SGDBMetadataSettings newSettings)
        {
            Client?.UpdateSettings(newSettings);
        }

        public override OnDemandMetadataProvider GetMetadataProvider(MetadataRequestOptions options)
        {
            return new SGDBMetadataProvider(options, this, settings.Settings, Client);
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SGDBMetadataSettingsView();
        }
    }
}
