
using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine
{
    class GameRenamer
    {
        private static readonly Regex cleaner = new Regex(@" *\([^)]*\) *", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //////////////////////////////////////////////////
        public static void renameSelectedGames(Boolean extraInfo)
        {
            // Rename all selected Playnite games
            int renamedGames = 0;
            UI.UIService.showSelectedGamesProgress("Renaming selection", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);

                if (mameMachine != null && mameMachine.isGame())
                {
                    renameGame(game, mameMachine, extraInfo);
                    renamedGames++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{renamedGames} games were renamed.");
        }

        //////////////////////////////////////////////////
        private static bool renameGame(Game playniteGame, RomsetMachine mameMachine, Boolean extraInfo)
        {
            string newName = (extraInfo) ? mameMachine.description : cleaner.Replace(mameMachine.description, "");

            // Only tell the database to update this game if the name actually changed
            if (!playniteGame.Name.Equals(newName))
            {
                playniteGame.Name = newName;
                MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
            }
            return true;
        }
    }
}
