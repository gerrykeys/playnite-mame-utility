using MAMEUtility.Models;
using MAMEUtility.Services.Cache;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms;
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
            public static string Bios = "Bios";
            public static string Device = "Device";
            public static string Sample = "Sample";
            public static string Mechanical = "Mechanical";
            public static string Parent = "Parent";
            public static string Clone = "Clone";
        }

        ////////////////////////////////////////////////////////////////////
        public static void setTagOfSelectedGames()
        {
            // Tag selected Playnite games
            int taggedCount = 0;
            UI.UIService.showSelectedGamesProgress("Tagging selection", (game, machines, progressArgs) =>
            {
                RomsetMachine mameMachine = MachinesService.findMachineByPlayniteGame(machines, game);
                if (mameMachine != null)
                {
                    tagGame(game, mameMachine);
                    taggedCount++;
                }
            });

            // Show result message
            UI.UIService.showMessage($"{taggedCount} games were tagged");
        }

        //////////////////////////////////////////////////
        private static void tagGame(Game playniteGame, RomsetMachine mameMachine)
        {
            List<string> tagsToSet = new List<string>();

            if (mameMachine.isBios) tagsToSet.Add(Tags.Bios);
            if (mameMachine.isDevice) tagsToSet.Add(Tags.Device);
            if (mameMachine.isSample()) tagsToSet.Add(Tags.Sample);
            if (mameMachine.isMechanical) tagsToSet.Add(Tags.Mechanical);
            if (mameMachine.isGame() && !mameMachine.isClone()) tagsToSet.Add(Tags.Parent);
            if (mameMachine.isClone()) tagsToSet.Add(Tags.Clone);

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
                if (playniteTag.Name == tagName)
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
