﻿using MAMEUtility.Models;
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
        //////////////////////////////////////////////////
        public static void removeSelectedNonGames()
        {
            // Get machines
            string sourceListFileType = MAMEUtilityPlugin.settings.Settings.SelectedRomsetSourceFormat;
            Dictionary<string, RomsetMachine> machines = MachinesService.getMachines();
            if (machines == null)
            {
                UI.UIService.showError("No machines found", "Cannot get Machines. Please check plugin settings.");
                return;
            }

            // Remove selected non-Game
            int nonGamesRemoved = 0;
            int selectedGamesCount = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Removing non-Games (bios/device/sample) from selection", false, true, (progressAction) => {
                
                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
                selectedGamesCount = selectedGames.Count();
                foreach (Game game in selectedGames)
                {
                    RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                    if(mameMachine != null && !mameMachine.isGame())
                    {
                        removeGame(game);
                        nonGamesRemoved++;
                    }
                }
            });

            if (selectedGamesCount == 0)
            {
                UI.UIService.showMessage("No games selected. Please select games.");
                return;
            }

            // Show result message
            UI.UIService.showMessage(nonGamesRemoved + " non-Games were removed from library");
        }


        //////////////////////////////////////////////////
        public static void removeSelectedCloneGames()
        {
            // Get machines
            string sourceListFileType = MAMEUtilityPlugin.settings.Settings.SelectedRomsetSourceFormat;
            Dictionary<string, RomsetMachine> machines = MachinesService.getMachines();
            if (machines == null)
            {
                UI.UIService.showError("No machines found", "Cannot get Machines. Please check plugin settings.");
                return;
            }

            // Remove selected clone games
            int cloneGames = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Removing clone Games from selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
                foreach (Game game in selectedGames)
                {
                    RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                    if (mameMachine != null && mameMachine.isClone())
                    {
                        removeGame(game);
                        cloneGames++;
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(cloneGames + " Clone Games were removed from library");
        }

        //////////////////////////////////////////////////
        public static void removeSelectedMechanicalGames()
        {
            // Get machines
            string sourceListFileType = MAMEUtilityPlugin.settings.Settings.SelectedRomsetSourceFormat;
            Dictionary<string, RomsetMachine> machines = MachinesService.getMachines();
            if (machines == null)
            {
                UI.UIService.showError("No machine found", "Cannot get Machines. Please check plugin settings.");
                return;
            }

            // Remove mechanical games from selection
            int mechanicalGames = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Removing mechanical Games from selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
                foreach (Game game in selectedGames)
                {
                    RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                    if (mameMachine != null && mameMachine.isMechanical)
                    {
                        removeGame(game);
                        mechanicalGames++;
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(mechanicalGames + " Mechanical Games were removed from library");
        }

        //////////////////////////////////////////////////
        private static void removeGame(Game playniteGame)
        {
            MAMEUtilityPlugin.playniteAPI.Database.Games.Remove(playniteGame.Id);
        }
    }
}
