using MAMEUtility.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine.Platforms.FBNeo
{
    class FBNeoMachinesService
    {
        ////////////////////////////////////////////////////////////////////////////////
        public static MachinesResponseData getMachines()
        {
            MachinesResponseData responseData = new MachinesResponseData();

            // Case of no cached data
            if (Cache.DataCache.mameMachines.Count == 0)
            {
                // Generate MAME machines
                responseData.machines = getMachinesFromCache();
                return responseData;
            }

            // Case of cached data: Ask to user if wants to use cached data or regenerate machines
            DialogResult dlgResult = UI.UIService.openAskDialog("", "Romset Machines was previously generated. Do you want to use cached data? If no, then a rescan will be launched");
            if (dlgResult == DialogResult.Cancel)
            {
                responseData.isOperationCancelled = true;
                return responseData;
            }
            if (dlgResult == DialogResult.Yes)
            {
                responseData.machines = Cache.DataCache.mameMachines;
                return responseData;
            }

            responseData.machines = getMachinesFromMameListFile(MAMEUtilityPlugin.settings.Settings.SourceListFilePath);
            return responseData;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, RomsetMachine> getMachinesFromMameListFile(string mameListFilePath)
        {
            if (string.IsNullOrEmpty(mameListFilePath))
            {
                UI.UIService.showError("Missing DAT source list file", "You are using list file as source but you have not set the file path from extension settings");
                return null;
            }

            // Get gamelist from MAME executable
            Dictionary<string, RomsetMachine> mameMachines = new Dictionary<string, RomsetMachine>();
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Machines from FBNeo source file", false, true, (progressAction) => {
                mameMachines = FBNeoMachinesFileLoader.getMachinesFromSourceListFile(mameListFilePath);
            });

            return mameMachines;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, RomsetMachine> getMachinesFromCache()
        {
            return Cache.DataCache.mameMachines;
        }
    }
}
