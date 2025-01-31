using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;

namespace MAMEUtility.Services.Engine
{
    class GameCleaner
    {
        /// <summary>
        /// Removes games from selection that are not playable like bios/device/sample.
        /// </summary>
        public static void removeSelectedNonGames()
        {
            int nonGamesRemoved = 0;
            UI.UIService.showSelectedGamesProgress("Removing non-games (bios/device/sample) from selection", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                if (mameMachine != null && !mameMachine.isGame())
                {
                    removeGame(game);
                    nonGamesRemoved++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{nonGamesRemoved} non-games were removed from library.");
        }


        /// <summary>
        /// Removes games from selection that are clones.
        /// </summary>
        public static void removeSelectedCloneGames()
        {
            int cloneGamesRemoved = 0;
            UI.UIService.showSelectedGamesProgress("Removing clone games from selection", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                if (mameMachine != null && mameMachine.isClone())
                {
                    removeGame(game);
                    cloneGamesRemoved++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{cloneGamesRemoved} clone games were removed from library.");
        }

        /// <summary>
        /// Removes games from selection that are mechanical.
        /// </summary>
        public static void removeSelectedMechanicalGames()
        {
            int mechanicalGamesRemoved = 0;
            UI.UIService.showSelectedGamesProgress("Removing mechanical games from selection", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                if (mameMachine != null && mameMachine.isMechanical)
                {
                    removeGame(game);
                    mechanicalGamesRemoved++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{mechanicalGamesRemoved} mechanical games were removed from library.");
        }

        /// <summary>
        /// Removes the given game from the Playnite games database.
        /// </summary>
        /// <param name="playniteGame">Game to remove</param>
        private static void removeGame(Game playniteGame)
        {
            MAMEUtilityPlugin.playniteAPI.Database.Games.Remove(playniteGame.Id);
        }
    }
}
