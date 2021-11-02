using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAMEUtility.Services.Engine
{
    class GameMediaManager
    {
        ////////////////////////////////////////////////////
        public static void setCoverImagesOfSelectedGames()
        {
            // Ask user to select the directory for images
            string scanDirectory = UI.UIService.openDirectoryDialogChooser();
            if (string.IsNullOrEmpty(scanDirectory))
                return;

            // Get all files in directory
            string[] filePathList = Directory.GetFiles(scanDirectory);
            Dictionary<string, string> fileListMap = new Dictionary<string, string>();
            foreach (string fpath in filePathList)
            {
                string fileNameWithExtension = Path.GetFileName(fpath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(fileNameWithExtension);
                fileListMap.Add(fileName, fpath);
            }

            // Apply images for all selected Playnite games
            int processedGames = 0;
            int coverApplyed   = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Applying cover images to selection", false, true, (progressAction) => {
                
                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
                foreach (Game game in selectedGames)
                {
                    processedGames++;

                    if (game.Roms.Count <= 0)
                        return;

                    if(fileListMap.ContainsKey(game.Roms[0].Name))
                    {
                        try
                        {
                            // if game has a cover image, then remove it
                            if(game.CoverImage != null)
                            {
                                MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.CoverImage);
                            }

                            // Add cover image
                            Guid guid       = Guid.NewGuid();
                            string filepath = fileListMap[game.Roms[0].Name];
                            string id       = MAMEUtilityPlugin.playniteAPI.Database.AddFile(filepath, guid);
                            game.CoverImage = id;
                            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
                            coverApplyed++;
                        }
                        catch (Exception){}
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(coverApplyed + " Cover images were set");
        }

        ////////////////////////////////////////////////////
        public static void setBackgroundImagesOfSelectedGames()
        {
            // Ask user to select the directory for images
            string scanDirectory = UI.UIService.openDirectoryDialogChooser();
            if (string.IsNullOrEmpty(scanDirectory))
                return;

            // Get all files in directory
            string[] filePathList = Directory.GetFiles(scanDirectory);
            Dictionary<string, string> fileListMap = new Dictionary<string, string>();
            foreach (string fpath in filePathList)
            {
                string fileNameWithExtension = Path.GetFileName(fpath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(fileNameWithExtension);
                fileListMap.Add(fileName, fpath);
            }

            // Apply background image for all selected Playnite games
            int processedGames    = 0;
            int backgroundApplyed = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Applying background images to selection", false, true, (progressAction) => {

                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
                foreach (Game game in selectedGames)
                {
                    processedGames++;

                    if (game.Roms.Count <= 0)
                        return;

                    if (fileListMap.ContainsKey(game.Roms[0].Name))
                    {
                        try
                        {
                            // if game has a cover image, then remove it
                            if(game.BackgroundImage != null)
                            {
                                MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.BackgroundImage);
                            }

                            // Add background image
                            Guid guid = Guid.NewGuid();
                            string filepath = fileListMap[game.Roms[0].Name];
                            string id = MAMEUtilityPlugin.playniteAPI.Database.AddFile(filepath, guid);
                            game.BackgroundImage = id;
                            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
                            backgroundApplyed++;
                        }
                        catch (Exception) { }
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(backgroundApplyed + " Background images were set");
        }
    }
}
