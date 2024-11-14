
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
            // Get machines
            Dictionary<string, RomsetMachine> machines = MachinesService.getMachines();
            if (machines == null) {
                UI.UIService.showError("No machine found", "Cannot get Machines. Please check plugin settings.");
                return;
            }

            // Rename all selected Playnite games
            int processedCount = 0;
            int renamedCount   = 0;
            int selectedGamesCount = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Renaming selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Get selected games count
                selectedGamesCount = selectedGames.Count();

                // Rename machines
                foreach (Game game in selectedGames)
                {
                    RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                    if(mameMachine.romName == "buckrogn")
                    {
                        int a = 0;
                    }     
                    if (mameMachine != null && mameMachine.isGame())
                    {
                        renameGame(game, mameMachine, extraInfo);
                        renamedCount++;
                    }

                    processedCount++;
                }
            });

            if(selectedGamesCount == 0)
            {
                UI.UIService.showMessage("No games selected. Please select games.");
                return;
            }

            // Show result message
            UI.UIService.showMessage(renamedCount + " Games were renamed");
        }

        //////////////////////////////////////////////////
        private static bool renameGame(Game playniteGame, RomsetMachine mameMachine, Boolean extraInfo)
        {
            if (extraInfo) {
                playniteGame.Name = mameMachine.description;
            }
            else {
                playniteGame.Name = cleaner.Replace(mameMachine.description, "");
            }
            
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
            return true;
        }
    }
}
