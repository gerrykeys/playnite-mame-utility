using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using Playnite.SDK.Models;
using Humanizer;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace MAMEUtility.Services.UI
{
    class UIService
    {
        /// <summary>
        /// Activates a progress dialog for the currently selected games and processed each game with the given handler.
        /// </summary>
        /// <param name="text">Dialog message</param>
        /// <param name="handlerForSingleGame">Function called for each selected game that is in charge of processing it.</param>
        public static void showSelectedGamesProgress(string text, Action<Game, Dictionary<string, RomsetMachine>, GlobalProgressActionArgs> handlerForSingleGame)
        {
            // Get selected games
            var selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;
            var selectedGamesCount = selectedGames.Count();

            if (selectedGamesCount == 0)
            {
                showMessage("No games selected. Please select games.");
                return;
            }

            // Get machines
            Dictionary<string, RomsetMachine> machines = MachinesService.getMachines();
            if (machines == null)
            {
                showError("No machines found", "Cannot get Machines. Please check plugin settings.");
                return;
            }

            // Use bulk updates as recommended by docs
            // https://api.playnite.link/docs/tutorials/extensions/library.html?tabs=csharp
            MAMEUtilityPlugin.playniteAPI.Database.Games.BeginBufferUpdate();
            MAMEUtilityPlugin.playniteAPI.Database.Tags.BeginBufferUpdate();

            // Process all selected games
            var options = new GlobalProgressOptions(text, true)
            {
                IsIndeterminate = false
            };
            MAMEUtilityPlugin.playniteAPI.Dialogs.ActivateGlobalProgress((progressArgs) =>
            {
                var elapsedTime = TimeSpan.Zero;
                progressArgs.ProgressMaxValue = selectedGamesCount;
                try
                {
                    foreach (Game game in selectedGames)
                    {
                        // User requested cancellation
                        if (progressArgs.CancelToken.IsCancellationRequested)
                            break;

                        // Calculate ETA based on average iteration time
                        if (progressArgs.CurrentProgressValue > 0)
                        {
                            var averageLoopMS = elapsedTime.TotalMilliseconds / progressArgs.CurrentProgressValue;
                            var remainingLoops = (progressArgs.ProgressMaxValue - progressArgs.CurrentProgressValue);
                            var eta = TimeSpan.FromMilliseconds(averageLoopMS * remainingLoops);
                            progressArgs.Text = $"{text} ({progressArgs.CurrentProgressValue}/{progressArgs.ProgressMaxValue}) \nETA: {eta.Humanize(4)}";
                        }

                        // Process this single game and time how long it takes to process
                        var stopwatch = Stopwatch.StartNew();
                        handlerForSingleGame(game, machines, progressArgs);
                        elapsedTime += stopwatch.Elapsed;

                        progressArgs.CurrentProgressValue++;
                    }

                }
                catch (Exception ex)
                {
                    showError($"Mame Utility Plugin Error: {ex.Message}", ex.StackTrace);
                    MAMEUtilityPlugin.logger.Error(ex.Message);
                    MAMEUtilityPlugin.logger.Error(ex.StackTrace);
                }
            }, options);


            // Send bulk updates to all listeners and wait for the database to finish updating
            var updateOptions = new GlobalProgressOptions("Updating Playnite database and saving changes to disk. \nPlease wait patiently as this operation could take a while if you selected a lot of games.", false)
            {
                IsIndeterminate = true,
            };
            MAMEUtilityPlugin.playniteAPI.Dialogs.ActivateGlobalProgress((args) =>
            {
                MAMEUtilityPlugin.playniteAPI.Database.Games.EndBufferUpdate();
                MAMEUtilityPlugin.playniteAPI.Database.Tags.EndBufferUpdate();
            }, updateOptions);
        }

        //////////////////////////////////////////////////////////////////
        public static GlobalProgressResult showProgress(string text, bool cancelable, bool indeterminate, System.Action<GlobalProgressActionArgs> f)
        {
            GlobalProgressOptions options = new GlobalProgressOptions(text, cancelable)
            {
                IsIndeterminate = indeterminate
            };

            return MAMEUtilityPlugin.playniteAPI.Dialogs.ActivateGlobalProgress(f, options);
        }

        //////////////////////////////////////////////////////////////////
        public static void showMessage(string text)
        {
            MAMEUtilityPlugin.playniteAPI.Dialogs.ShowMessage(text);
        }

        //////////////////////////////////////////////////////////////////
        public static void showError(string title, string text)
        {
            MAMEUtilityPlugin.playniteAPI.Dialogs.ShowErrorMessage(text, title);
        }

        //////////////////////////////////////////////////////////////////
        public static DialogResult openAskDialog(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        //////////////////////////////////////////////////////////////////
        public static string openFileDialogChooser(string extensionFiltersType)
        {
            return MAMEUtilityPlugin.playniteAPI.Dialogs.SelectFile(extensionFiltersType);
        }

        //////////////////////////////////////////////////////////////////
        public static string openDirectoryDialogChooser()
        {
            return MAMEUtilityPlugin.playniteAPI.Dialogs.SelectFolder();
        }
    }
}
