using MAMEUtility.Services.Engine;
using MAMEUtility.Services.Engine.Library;
using Playnite.SDK.Plugins;
using System.Collections.Generic;

namespace MAMEUtility.Configuration
{
    class MainMenu
    {
        ////////////////////////////////////////////////////////////
        public static List<MainMenuItem> getPluginMenuItems()
        {
            return new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName,
                    Description = "Rename Selected Games",
                    Action = (args) => GameRenamer.renameSelectedGames(true)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName,
                    Description = "Rename Selected Games (Without Extra Information in Name)",
                    Action = (args) => GameRenamer.renameSelectedGames(false)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set cover images of selected Games",
                    Action = (args) => GameMediaManager.setImageOfSelectedGames(GameMediaManager.ImageType.Cover)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set background images of selected Games",
                    Action = (args) => GameMediaManager.setImageOfSelectedGames(GameMediaManager.ImageType.Background)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set icons of selected Games",
                    Action = (args) => GameMediaManager.setImageOfSelectedGames(GameMediaManager.ImageType.Icon)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set (extrametadata) videos of selected Games",
                    Action = (args) => GameMediaManager.setExtraMetaDataOfSelectedGames(GameMediaManager.ExtraMetaDataType.Video)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set (extrametadata) micro videos of selected Games",
                    Action = (args) => GameMediaManager.setExtraMetaDataOfSelectedGames(GameMediaManager.ExtraMetaDataType.MicroVideo)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Media",
                    Description = "Set (extrametadata) logo of selected Games",
                    Action = (args) => GameMediaManager.setExtraMetaDataOfSelectedGames(GameMediaManager.ExtraMetaDataType.Logo)
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Cataloguer",
                    Description = "Tag selected games",
                    Action = (args) => GameTagger.setTagOfSelectedGames()
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Cleaner",
                    Description = "Remove non-Games from selection",
                    Action = (args) => GameCleaner.removeSelectedNonGames()
                },
                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Cleaner",
                    Description = "Remove clone Games from selection",
                    Action = (args) => GameCleaner.removeSelectedCloneGames()
                },
                                new MainMenuItem
                {
                    MenuSection = "@" + MAMEUtilityPlugin.PluginName + "|Cleaner",
                    Description = "Remove mechanical Games from selection",
                    Action = (args) => GameCleaner.removeSelectedMechanicalGames()
                },
            };
        }
    }
}
