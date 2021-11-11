
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
        public static void renameSelectedGames(Boolean extraInfo)
        {
            // Get machines
            MachinesResponseData responseData = MachinesService.getMachines();
            if (responseData.isOperationCancelled) return;
            if (responseData.machines == null) {
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
                        renameGame(game, mameMachine, extraInfo);
                        renamedCount++;
                    }

                    processedCount++;
                }
            });

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
                playniteGame.Name = mameMachine.description.Remove(mameMachine.description.LastIndexOf("(") - 1);
            }
            
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
            return true;
        }
    }
}
