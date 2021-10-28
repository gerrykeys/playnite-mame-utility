
using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MAMEUtility.Services.Engine
{
    class GameRenamer
    {
        //////////////////////////////////////////////////
        public static void renameSelectedGames()
        {
            // Get machines
            MachinesResponseData responseData = MAMEMachinesService.getMachines();
            if (responseData.isOperationCancelled) return;
            if(responseData.machines == null) {
                UI.UIService.showError("No machine founds", "Cannot get Machines. Please check extension settings.");
                return;
            }

            // Rename all selected Playnite games
            int processedCount = 0;
            int renamedCount   = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Renaming selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Rename machines
                foreach (Game game in selectedGames)
                {
                    RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(responseData.machines, game);
                    if (mameMachine != null && mameMachine.isGame())
                    {
                        renameGame(game, mameMachine);
                        renamedCount++;
                    }

                    processedCount++;
                }
            });

            // Show result message
            UI.UIService.showMessage(renamedCount + " Games were renamed");
        }

        //////////////////////////////////////////////////
        private static bool renameGame(Game playniteGame, RomsetMachine mameMachine)
        {
            playniteGame.Name = mameMachine.description;
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
            return true;
        }
    }
}
