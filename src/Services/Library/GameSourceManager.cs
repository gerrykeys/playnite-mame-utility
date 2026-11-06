using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MAMEUtility.Services.Engine.Library
{
    class GameSourceManager
    {
        public static void assignSourceOfSelectedGames()
        {
            int assignedCount = 0;

            UI.UIService.showSelectedGamesProgress("Assigning ROM sources", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                if (mameMachine != null && !string.IsNullOrEmpty(mameMachine.sourceFile))
                {
                    // Playnite's Game.Source is a read-only convenience property.
                    // The actual value is stored through SourceId.
                    GameSource source = MAMEUtilityPlugin.playniteAPI.Database.Sources
                        .FirstOrDefault(s => s.Name == mameMachine.sourceFile);

                    if (source == null)
                    {
                        // Add(string) creates the GameSource and returns the
                        // existing one if another source with the same name exists.
                        source = MAMEUtilityPlugin.playniteAPI.Database.Sources.Add(mameMachine.sourceFile);
                    }

                    if (game.SourceId != source.Id)
                    {
                        game.SourceId = source.Id;
                        MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
                        assignedCount++;
                    }
                }
            });

            UI.UIService.showMessage($"{assignedCount} games were assigned a ROM source");
        }

        public static void removePluginTagsAndSources()
        {
            int cleanedCount = 0;

            UI.UIService.showSelectedGamesProgress("Removing MAME Utility tags and sources", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                bool changed = false;

                if (game.TagIds != null)
                {
                    Guid[] pluginTagIds =
                    {
                        GameTagger.getTagIdForCleanup("Bios"),
                        GameTagger.getTagIdForCleanup("Device"),
                        GameTagger.getTagIdForCleanup("Sample"),
                        GameTagger.getTagIdForCleanup("Mechanical"),
                        GameTagger.getTagIdForCleanup("Parent"),
                        GameTagger.getTagIdForCleanup("Clone")
                    };

                    foreach (Guid tagId in pluginTagIds)
                    {
                        if (tagId != System.Guid.Empty && game.TagIds.Remove(tagId))
                            changed = true;
                    }
                }

                // There is no provenance field in Playnite for Source.
                // Only remove it when it matches the sourcefile from the MAME XML,
                // so manually assigned sources are preserved.
                if (mameMachine != null &&
                    game.SourceId != Guid.Empty &&
                    !string.IsNullOrEmpty(mameMachine.sourceFile) &&
                    game.Source != null &&
                    game.Source.Name == mameMachine.sourceFile)
                {
                    game.SourceId = Guid.Empty;
                    changed = true;
                }

                if (changed)
                {
                    MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
                    cleanedCount++;
                }
            });

            UI.UIService.showMessage($"{cleanedCount} games were cleaned");
        }
    }
}
