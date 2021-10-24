using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAMEUtility.Services.Engine.Library
{
    class GameTagger
    {
        ////////////////////////////////////////////////////////////////////
        class Tags
        {
            public static string Bios       = "Bios";
            public static string Device     = "Device";
            public static string Sample     = "Sample";
            public static string Mechanical = "Mechanical";
            public static string Parent     = "Parent";
            public static string Clone      = "Clone";
        }

        ////////////////////////////////////////////////////////////////////
        public static void setTagOfSelectedGames()
        {
            bool isOperationCanceled = false;
            // Get MAME machines
            Dictionary<string, MAMEMachine> mameMachines = MachineService.getMachines(ref isOperationCanceled);
            if (isOperationCanceled) return;
            if (mameMachines == null) {
                UI.UIService.showError("No machine founds", "Cannot get Gamelist from MAME executable");
                return;
            }

            // Tag selected Playnite games
            int taggedCount = 0;
            GlobalProgressResult progressResult = UI.UIService.showProgress("Tagging selection", false, true, (progressAction) => {

                // Get selected games
                IEnumerable<Game> selectedGames = MAMEUtilityPlugin.playniteAPI.MainView.SelectedGames;

                // Rename only game machines
                foreach (Game game in selectedGames)
                {
                    MAMEMachine mameMachine = DataCache.findMachineByPlayniteGame(game);
                    if (mameMachine != null)
                    {
                        tagGame(game, mameMachine);
                        taggedCount++;
                    }
                }
            });

            // Show result message
            UI.UIService.showMessage(taggedCount + " Games were tagged");
        }

        //////////////////////////////////////////////////
        private static void tagGame(Game playniteGame, MAMEMachine mameMachine)
        {
            List<string> tagsToSet = new List<string>();

            if (mameMachine.isBios)                             tagsToSet.Add(Tags.Bios);
            if (mameMachine.isDevice)                           tagsToSet.Add(Tags.Device);
            if (mameMachine.isSample())                         tagsToSet.Add(Tags.Sample);
            if (mameMachine.isMechanical)                       tagsToSet.Add(Tags.Mechanical);
            if (mameMachine.isGame() && !mameMachine.isClone()) tagsToSet.Add(Tags.Parent);
            if (mameMachine.isClone())                          tagsToSet.Add(Tags.Clone);

            if (tagsToSet.Count == 0) return;

            foreach (string tagStr in tagsToSet)
            {
                if (playniteGame.TagIds == null) 
                        playniteGame.TagIds = new List<Guid>();
                
                playniteGame.TagIds.AddMissing<Guid>(getTagId(tagStr));
            }

            MAMEUtilityPlugin.playniteAPI.Database.Games.Update(playniteGame);
        }

        //////////////////////////////////////////////////
        private static Guid getTagId(string tagName)
        {
            Guid tagId = Guid.Empty;
            IItemCollection<Tag> playniteTags = MAMEUtilityPlugin.playniteAPI.Database.Tags;
            foreach (Tag playniteTag in playniteTags)
            {
                if(playniteTag.Name == tagName)
                {
                   return playniteTag.Id;
                }
            }

            // we have to add tag in playnite database
            Tag tag = new Tag(tagName);
            MAMEUtilityPlugin.playniteAPI.Database.Tags.Add(tag);
            return tag.Id;
        }
    }
}
