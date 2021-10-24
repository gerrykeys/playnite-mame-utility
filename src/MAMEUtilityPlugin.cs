using MAMEUtility.Configuration;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MAMEUtility
{
    public class MAMEUtilityPlugin : GenericPlugin
    {
        public override Guid Id { get; } = Guid.Parse("af854071-4cd2-47ae-a7b3-35c238f0f7c0");
        private static readonly ILogger logger = LogManager.GetLogger();

        //////////////////////////////////////////////////////////////////////////////////////
        public static string PluginName = "MAME Utility";

        //////////////////////////////////////////////////////////////////////////////////////
        public static IPlayniteAPI playniteAPI;

        //////////////////////////////////////////////////////////////////////////////////////
        public static MAMEUtilitySettingsViewModel settings { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////
        public MAMEUtilityPlugin(IPlayniteAPI api) : base(api)
        {
            playniteAPI = api;
            settings = new MAMEUtilitySettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        //////////////////////////////////////////////////////////////////////////////////////
        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs _args)
        {
            return MainMenu.getPluginMenuItems();
        }

        //////////////////////////////////////////////////////////////////////////////////////
        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new MAMEUtilitySettingsView(this);
        }
    }
}