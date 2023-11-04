using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace MAMEUtility.Services.Engine
{
    class GameMediaManager
    {
        ////////////////////////////////////////////////////
        public enum ImageType { Cover, Background, Icon }

        ////////////////////////////////////////////////////
        public enum ExtraMetaDataType { Logo, Video, MicroVideo }

        ////////////////////////////////////////////////////
        private const string longPathPrefix = @"\\?\";

        ////////////////////////////////////////////////////
        private const string longPathUncPrefix = @"\\?\UNC\";

        ////////////////////////////////////////////////////
        private static Dictionary<string, string> imageFileListMap = new Dictionary<string, string>();
        
        ////////////////////////////////////////////////////        
        private static bool IsFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            // Don't use Path.IsPathRooted because it fails on paths starting with one backslash.
            return Regex.IsMatch(path, @"^([a-zA-Z]:\\|\\\\)");
        }

        ////////////////////////////////////////////////////
        private static string FixPathLength(string path)
        {
            // Relative paths don't support long paths
            // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=cmd
            if (!IsFullPath(path))
            {
                return path;
            }

            if (path.Length >= 258 && !path.StartsWith(longPathPrefix))
            {
                if (path.StartsWith(@"\\"))
                {
                    return longPathUncPrefix + path.Substring(2);
                }
                else
                {
                    return longPathPrefix + path;
                }
            }

            return path;
        }

        ////////////////////////////////////////////////////
        private static string GetExtraMetadataDirectory(Game game, bool createDirectory = false)
        {
            var directory = Path.Combine(FixPathLength(Path.Combine(MAMEUtilityPlugin.playniteAPI.Paths.ConfigurationPath, "ExtraMetadata")), 
                "games", game.Id.ToString());
            if (createDirectory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        ////////////////////////////////////////////////////
        private static string GetGameLogoPath(Game game, bool createDirectory = false)
        {
            return Path.Combine(GetExtraMetadataDirectory(game, createDirectory), "Logo.png");
        }

        ////////////////////////////////////////////////////
        private static string GetGameVideoPath(Game game, bool createDirectory = false)
        {
            return Path.Combine(GetExtraMetadataDirectory(game, createDirectory), "VideoTrailer.mp4");
        }

        ////////////////////////////////////////////////////
        private static string GetGameVideoMicroPath(Game game, bool createDirectory = false)
        {
            return Path.Combine(GetExtraMetadataDirectory(game, createDirectory), "VideoMicrotrailer.mp4");
        }

        ////////////////////////////////////////////////////
        public static void setExtraMetaDataOfSelectedGames(ExtraMetaDataType mediaType)
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
            int imagesApplied = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Applying extrametadata videos to selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Apply images for each game
                foreach (Game game in selectedGames)
                {
                    // find an image for the game
                    string imageFilePath = findImage(game);

                    // if image is found then set to game
                    if (!string.IsNullOrEmpty(imageFilePath))
                    {
                        setGameExtraMetaData(mediaType, game, imageFilePath);
                        imagesApplied++;
                    }
                }
            });
            switch (mediaType)
            {
                case ExtraMetaDataType.Logo:
                    UI.UIService.showMessage(imagesApplied + " logos were set");
                    break;

                case ExtraMetaDataType.MicroVideo:
                case ExtraMetaDataType.Video:
                    UI.UIService.showMessage(imagesApplied + " videos were set");
                    break;
            }
            
        }

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
            if (imageType == ImageType.Cover && game.CoverImage != null) MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.CoverImage);
            if (imageType == ImageType.Background && game.BackgroundImage != null) MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.BackgroundImage);
            if (imageType == ImageType.Icon && game.Icon != null) MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(game.Icon);

            // add image to playnite database
            Guid guid = Guid.NewGuid();
            string id = MAMEUtilityPlugin.playniteAPI.Database.AddFile(imageFilePath, guid);

            // assign image to game
            if      (imageType == ImageType.Cover) game.CoverImage = id;
            else if (imageType == ImageType.Background) game.BackgroundImage = id;
            else if (imageType == ImageType.Icon) game.Icon = id;

            // update game
            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(game);
        }

        ////////////////////////////////////////////////////
        private static void setGameExtraMetaData(ExtraMetaDataType mediaType, Game game, string mediaFilePath)
        {
            try
            {
                switch (mediaType)
                {
                    case ExtraMetaDataType.Logo:
                        File.Copy(mediaFilePath, GetGameLogoPath(game, true));
                        break;
                    case ExtraMetaDataType.MicroVideo:
                        File.Copy(mediaFilePath, GetGameVideoMicroPath(game, true));
                        break;
                    case ExtraMetaDataType.Video:
                        File.Copy(mediaFilePath, GetGameVideoPath(game, true));
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
