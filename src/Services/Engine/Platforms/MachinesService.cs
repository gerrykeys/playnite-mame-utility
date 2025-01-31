using MAMEUtility.Models;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms.FBNeo;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine.Platforms
{
    //////////////////////////////////////////////////////////////
    public enum SourceType { Unknow, MAMEExecutable, DATFile }

    //////////////////////////////////////////////////////////////
    public enum SourceFormat { Unknow, MAME, FBNeo }

    //////////////////////////////////////////////////////////////
    class MachinesService
    {
        //////////////////////////////////////////////////////////////
        private static readonly Regex extensionCleaner = new Regex(@"\.[^.]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //////////////////////////////////////////////////////////////
        public static SourceType getSourceType()
        {
            return (MAMEUtilityPlugin.settings.Settings.UseMameExecutable) ? SourceType.MAMEExecutable : SourceType.DATFile;
        }

        //////////////////////////////////////////////////////////////
        public static SourceFormat getSourceFormatByString(string str)
        {
            if (str == "MAME") return SourceFormat.MAME;
            if (str == "FBNeo") return SourceFormat.FBNeo;
            return SourceFormat.Unknow;
        }

        //////////////////////////////////////////////////////////////
        public static SourceFormat getSourceFormat()
        {
            SourceType sourceType = getSourceType();
            SourceFormat sourceFormat = SourceFormat.Unknow;
            if (sourceType == SourceType.MAMEExecutable)
            {
                sourceFormat = SourceFormat.MAME;
            }
            else
            {
                sourceFormat = getSourceFormatByString(MAMEUtilityPlugin.settings.Settings.SelectedRomsetSourceFormat);
            }
            return sourceFormat;
        }


        //////////////////////////////////////////////////////////////
        public static Dictionary<string, RomsetMachine> getMachines()
        {
            // Get source type
            SourceType sourceType = getSourceType();
            if (sourceType == SourceType.Unknow)
                return null;

            // Get format type
            SourceFormat sourceFormat = getSourceFormat();
            if (sourceFormat == SourceFormat.Unknow)
                return null;

            // If cache exists then ask to user it should be used or not
            if (Cache.DataCache.machines != null && Cache.DataCache.machines.Count > 0)
            {
                DialogResult dlgResult = UI.UIService.openAskDialog("", "Romset information was already generated. Do you want to use existing data? If no, data will be regenerated");
                if (dlgResult == DialogResult.Cancel)
                {
                    return null;
                }
                if (dlgResult == DialogResult.Yes)
                {
                    return Cache.DataCache.machines;
                }
            }

            // Otherwise generate machines
            Dictionary<string, RomsetMachine> machines = generateMachines(sourceFormat);

            // Update Cache
            Cache.DataCache.machines = machines;

            return machines;
        }

        //////////////////////////////////
        private static Dictionary<string, RomsetMachine> generateMachines(SourceFormat sourceFormat)
        {
            Dictionary<string, RomsetMachine> machines;

            // Get machines by machine type
            if (sourceFormat == SourceFormat.MAME) machines = MAMEMachinesService.getMachines();
            else if (sourceFormat == SourceFormat.FBNeo) machines = FBNeoMachinesService.getMachines();
            else return null;

            // sanity check
            if (machines == null) return null;

            // Link parent/clones
            linkParentClones(machines);

            return machines;
        }

        //////////////////////////////////////////////////////
        public static RomsetMachine findMachineByPlayniteGame(Dictionary<string, RomsetMachine> mameMachines, Game playniteGame)
        {
            if (mameMachines.ContainsKey(playniteGame.Name))
                return mameMachines[playniteGame.Name];

            if (playniteGame.Roms == null || playniteGame.Roms.Count == 0)
                return null;

            foreach (var rom in playniteGame.Roms)
            {
                string path = rom.Path;
                string nameCleaned = path.Substring(path.LastIndexOf("\\") + 1);

                nameCleaned = extensionCleaner.Replace(nameCleaned, "");

                if (mameMachines.ContainsKey(nameCleaned))
                    return mameMachines[nameCleaned];
            }

            return null;
        }

        //////////////////////////////////////////////////////
        private static void linkParentClones(Dictionary<string, RomsetMachine> machinesMap)
        {
            foreach (KeyValuePair<string, RomsetMachine> entry in machinesMap)
            {
                RomsetMachine machine = entry.Value;
                if (machine.isClone())
                {
                    string parent = machine.cloneOf;
                    string clone = entry.Key;
                    machinesMap[parent].clones.AddMissing<string>(clone);
                }
            }
        }
    }
}
