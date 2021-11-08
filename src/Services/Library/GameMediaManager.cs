using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MAMEUtility.Services.Engine
{
    class GameMediaManager
    {
        ////////////////////////////////////////////////////
        public enum ImageType { Cover, Background }

        ////////////////////////////////////////////////////
        private static Dictionary<string, string> imageFileListMap = new Dictionary<string, string>();

        ////////////////////////////////////////////////////
        public static void setImageOfSelectedGames(ImageType imageType)
        {
            // Get machines
            MachinesResponseData responseData = MachinesService.getMachines();
            if (responseData.isOperationCancelled) return;
            if (responseData.machines == null)
            {
                UI.UIService.showError("No machine founds", "Cannot get Machines. Please check extension settings.");
                return;
            }

            // Load file images
            loadFileImages();

            // Apply images for all selected Playnite games
            int imagesApplied   = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Applying images to selection", false, true, (progressAction) => {
                
                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Apply images for each game
                foreach (Game game in selectedGames)
                {
                    // find an image for the game
                    string imageFilePath = findImage(game);

                    // if image is found then set to game
                    if(!string.IsNullOrEmpty(imageFilePath))
                    {
                        setGameImage(imageType, game, imageFilePath);
                        imagesApplied++;
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(imagesApplied + " images were set");
        }

        ////////////////////////////////////////////////////
        private static void loadFileImages()
        {
            // Ask user to select the directory for images
            string scanDirectory = UI.UIService.openDirectoryDialogChooser();
            if (string.IsNullOrEmpty(scanDirectory))
                return;

            // Get all files in directory
            string[] filePathList = Directory.GetFiles(scanDirectory);
            imageFileListMap = new Dictionary<string, string>();
            foreach (string fpath in filePathList)
            {
                string fileNameWithExtension = Path.GetFileName(fpath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(fileNameWithExtension);
                imageFileListMap.Add(fileName, fpath);
            }
        }

        ////////////////////////////////////////////////////
        private static string findImage(Game game)
        {
            // if game has no rom file name then skip
            if (game.Roms.Count <= 0)
                return "";

            // get rom name
            string romName = game.Roms[0].Name;

            // try to find rom name in image file list map.
            // if exists then return the image file path
            if (imageFileListMap.ContainsKey(romName))
                return imageFileListMap[romName];

            // otherwise try to find in correlated
            return findImageInCorrelated(romName);
        }

        ////////////////////////////////////////////////////
        private static string findImageInCorrelated(string machineName)
        {
            // get the romset machine
            RomsetMachine machine = Cache.DataCache.mameMachines[machineName];

            // if machine is not a game then skip
            if (!machine.isGame())
                return "";

            // if the game is a parent, then add as correlated all its clones
            List<string> correlatedNames = new List<string>();
            if (machine.isParent())
            {
                correlatedNames = machine.clones;
            }

            // otherwise add as correlated the parent and all its clones
            else
            {
                // add its parent
                RomsetMachine parentMachine = Cache.DataCache.mameMachines[machine.cloneOf];
                correlatedNames.Add(parentMachine.romName);
                foreach (string clone in parentMachine.clones)
                {
                    if (machine.romName != clone)
                        correlatedNames.Add(clone);
                }
            }

            // try to find an image for a correlated game
            foreach (string correlatedName in correlatedNames)
            {
                if (imageFileListMap.ContainsKey(correlatedName))
                    return imageFileListMap[correlatedName];
            }

            return "";
        }

        ////////////////////////////////////////////////////
        private static void setGameImage(ImageType imageType, Game game, string imageFilePath)
        {
            // If game has just the image then remove it
            if(imageType == ImageType.Cover && game.CoverImage != null) MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.CoverImage);
            if (imageType == ImageType.Background && game.BackgroundImage != null) MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.BackgroundImage);

            // add image to playnite database
            Guid guid = Guid.NewGuid();
            string id = MAMEUtilityPlugin.playniteAPI.Database.AddFile(imageFilePath, guid);
            
            // assign image to game
            if (imageType == ImageType.Cover) game.CoverImage = id;
            else if (imageType == ImageType.Background) game.BackgroundImage = id;
            
            // update game
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
        }
    }
}
