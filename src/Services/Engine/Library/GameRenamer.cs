
using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
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
            // Get MAME machines
            bool isOperationCanceled = false;
            Dictionary<string, MAMEMachine> mameMachines = MachineService.getMachines(ref isOperationCanceled);
            if (isOperationCanceled) return;
            if(mameMachines == null) {
                UI.UIService.showError("No machine founds", "Cannot get Gamelist from MAME executable");
                return;
            }

            // Rename all selected Playnite games
            int processedCount = 0;
            int renamedCount   = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Renaming selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Rename only game machines
                foreach (Game game in selectedGames)
                {
                    MAMEMachine mameMachine = DataCache.findMachineByPlayniteGame(game);
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
        private static bool renameGame(Game playniteGame, MAMEMachine mameMachine)
        {
            playniteGame.Name = mameMachine.description;
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
            return true;
        }
    }
}
