using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static MAMEUtility.Services.Engine.GameMediaManager;

namespace MAMEUtility.Services.Engine
{
    class GameMediaManager
    {
        /// <summary>
        /// The types of images the manager can handle.
        /// </summary>
        public enum ImageType { Cover, Background, Icon }

        /// <summary>
        /// The types of extra metadata the manager can handle.
        /// </summary>
        public enum ExtraMetaDataType { Logo, Video, MicroVideo }

        ////////////////////////////////////////////////////
        private const string longPathPrefix = @"\\?\";

        ////////////////////////////////////////////////////
        private const string longPathUncPrefix = @"\\?\UNC\";

        /// <summary>
        /// A dictionary containing all the files of a given path.
        /// The keys are the filenames and the values are the complete filepaths.
        /// </summary>
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
            // Load file images
            loadFileImages();

            int imagesApplied = 0;
            UI.UIService.showSelectedGamesProgress($"Applying extrametadata ({mediaType}) to selection", (game, machines, progressArgs) =>
            {
                // find an image for the game
                string imageFilePath = findImage(game, machines);

                // if image is found then set to game
                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    setGameExtraMetaData(mediaType, game, imageFilePath);
                    imagesApplied++;
                }
            });

            switch (mediaType)
            {
                case ExtraMetaDataType.Logo:
                    UI.UIService.showMessage(imagesApplied + " matching logos were set");
                    break;
                case ExtraMetaDataType.MicroVideo:
                case ExtraMetaDataType.Video:
                    UI.UIService.showMessage(imagesApplied + " matching videos were set");
                    break;
            }

        }

        ////////////////////////////////////////////////////
        public static void setImageOfSelectedGames(ImageType imageType)
        {
            loadFileImages();

            int imagesApplied = 0;
            UI.UIService.showSelectedGamesProgress($"Applying images ({imageType}) to selection", (game, machines, progressArgs) =>
            {
                // find an image for the game
                string imageFilePath = findImage(game, machines);
                string imageName = Path.GetFileName(imageFilePath);

                // if image is found then set to game
                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    setGameImage(imageType, game, imageFilePath);
                    imagesApplied++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{imagesApplied} matching images were found and set.");
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
                string fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
                imageFileListMap.Add(fileName, fpath);
            }
        }

        ////////////////////////////////////////////////////
        private static string findImage(Game game, Dictionary<string, RomsetMachine> machines)
        {
            // We are going to have a list of candidate names to use as the image filename
            // Use a HashSet to not allow duplicates
            var namesToTry = new HashSet<string>();

            // Check roms
            if (game.Roms?.Count > 0)
            {
                // We'll try to match all rom names
                namesToTry.UnionWith(game.Roms.Select(r => r.Name));

                // We'll also try to match all the rom machine descriptions
                foreach (var roms in game.Roms)
                {
                    if (machines != null && machines.ContainsKey(roms.Name))
                        namesToTry.Add(machines[roms.Name].description);

                }
            }

            // Next check if this is a renamed rom, if so, add the original rom name to the names to try
            if (machines != null)
            {
                var matching = machines.Where(machine => machine.Value.description.Equals(game.Name, StringComparison.InvariantCultureIgnoreCase));
                if (matching.Any())
                    namesToTry.UnionWith(matching.Select(m => m.Value.romName));
            }

            // Lastly, just try the game name and version
            namesToTry.Add(game.Name);
            if (game.Version != null && game.Version != "")
                namesToTry.Add(game.Version);

            // For all possible filenames, try to find if an image with that filename exists
            foreach (var romName in namesToTry)
            {
                // try to find rom name in image file list map.
                // if exists then return the image file path
                if (imageFileListMap.ContainsKey(romName))
                    return imageFileListMap[romName];

                // otherwise try to find in correlated
                var correlated = findImageInCorrelated(romName, machines);
                if (correlated != null && correlated != "")
                    return correlated;
            }
            return "";
        }

        ////////////////////////////////////////////////////
        private static string findImageInCorrelated(string machineName, Dictionary<string, RomsetMachine> machines)
        {
            if (machines == null || !machines.ContainsKey(machineName)) return "";

            // get the romset machine
            RomsetMachine machine = machines[machineName];

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
                RomsetMachine parentMachine = machines[machine.cloneOf];
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

        /// <summary>
        /// Determines if two given image files are the same by hashing their content (MD5) and comparing the hashes.
        /// This is not a 100% method that will catch all files, but it is quite fast and works reasonably well.
        /// </summary>
        /// <param name="playniteImageFileID">The database ID for the playnite file to compare.</param>
        /// <param name="newImageFilePath">The filepath of the image to compare against.</param>
        /// <returns>True if the content for both given images produces the same MD5 hash.</returns>
        private static bool areImagesTheSame(string playniteImageFileID, string newImageFilePath)
        {
            if (playniteImageFileID == null || playniteImageFileID == "")
                return false;

            // Convert the playnite path to a full system path
            var playniteImageFullPath = MAMEUtilityPlugin.playniteAPI.Database.GetFullFilePath(playniteImageFileID);
            if (!File.Exists(playniteImageFullPath) || !File.Exists(newImageFilePath))
                return false;

            // Check if the image contents are the same by hashing them
            using (var md5 = MD5.Create())
            {
                var playniteImageHash = md5.ComputeHash(File.ReadAllBytes(playniteImageFullPath));
                var newImageHash = md5.ComputeHash(File.ReadAllBytes(newImageFilePath));
                return playniteImageHash.SequenceEqual(newImageHash);
            }
        }

        ////////////////////////////////////////////////////
        private static void setGameImage(ImageType imageType, Game game, string imageFilePath)
        {
            // Get the current image of the given type for the game
            string playniteImageFileID = null;
            if (imageType == ImageType.Cover) playniteImageFileID = game.CoverImage;
            else if (imageType == ImageType.Background) playniteImageFileID = game.BackgroundImage;
            else if (imageType == ImageType.Icon) playniteImageFileID = game.Icon;

            // The game already has an image of this type, check if it's the same as the new image
            if (playniteImageFileID != null && playniteImageFileID != "")
            {
                // If it's the same image, skip this game.
                // If it's different, delete the old image and proceed.
                if (areImagesTheSame(playniteImageFileID, imageFilePath))
                    return;
                else
                    MAMEUtilityPlugin.playniteAPI.Database.RemoveFile(playniteImageFileID);
            }

            // add image to playnite database
            Guid guid = Guid.NewGuid();
            string id = MAMEUtilityPlugin.playniteAPI.Database.AddFile(imageFilePath, guid);

            // assign image to game
            if (imageType == ImageType.Cover) game.CoverImage = id;
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
                MAMEUtilityPlugin.logger.Error(ex.Message);
                MAMEUtilityPlugin.logger.Error(ex.StackTrace);
            }
        }
    }
}
