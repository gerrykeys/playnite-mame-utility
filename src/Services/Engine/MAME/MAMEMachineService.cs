using MAMEUtility.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine.MAME
{
    class MAMEMachineService
    {
        ////////////////////////////////////////////////////////////////////////////////
        public static Dictionary<string, MAMEMachine> getMachines(ref bool isOperationCanceled)
        {
            // Case of no cached data
            if(Cache.DataCache.mameMachines.Count == 0)
            {
                // Generate MAME machines
                return (Cache.DataCache.mameMachines.Count == 0) ? generateMachines() : getMachinesFromCache();
            }

            // Case of cached data: Ask to user if wants to use cached data or regenerate MAME machines
            DialogResult dlgResult = UI.UIService.openAskDialog("", "MAME machines was previously generated. Do you want to use cached data? If no, then a rescan will be launched");
            if (dlgResult == DialogResult.Cancel) { isOperationCanceled = true; return null; }
            if (dlgResult == DialogResult.Yes) return Cache.DataCache.mameMachines;
            
            return generateMachines();
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, MAMEMachine> generateMachines()
        {
            var settings = MAMEUtilityPlugin.settings.Settings;

            // generate mame machines
            Dictionary<string, MAMEMachine> mameMachines =  (settings.UseMameExecutable) ? getMachinesFromMameExecutable(settings.MameExecutableFilePath) : getMachinesFromMameListFile(settings.MameListFilePath);

            // update cache
            Cache.DataCache.mameMachines = mameMachines;

            return mameMachines;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, MAMEMachine> getMachinesFromMameExecutable(string mameExecutablePath)
        {
            if (string.IsNullOrEmpty(mameExecutablePath))
            {
                MAMEUtilityPlugin.playniteAPI.Dialogs.ShowErrorMessage("Missing MAME executable", "You are using MAME executable as source but you have not set the MAME executable path from extension settings");
                return null;
            }

            // Get gamelist from MAME executable
            Dictionary<string, MAMEMachine> mameMachines = new Dictionary<string, MAMEMachine>();
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Gamelist data from MAME executable", false, true, (progressAction) => {
                mameMachines = MAMECliExecutor.getMachinesFromMameExecutable(mameExecutablePath);
            });

            return mameMachines;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, MAMEMachine> getMachinesFromMameListFile(string mameListFilePath)
        {
            if (string.IsNullOrEmpty(mameListFilePath))
            {
                UI.UIService.showError("Missing MAME source list file", "You are using MAME list file as source but you have not set the file path from extension settings");
                return null;
            }

            // Get gamelist from MAME executable
            Dictionary<string, MAMEMachine> mameMachines = new Dictionary<string, MAMEMachine>();
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Gamelist data from MAME executable", false, true, (progressAction) => {
                mameMachines = MAMEMachinesFileLoader.getMachinesFromListFile(mameListFilePath);
            });

            return mameMachines;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, MAMEMachine> getMachinesFromCache()
        {
            return Cache.DataCache.mameMachines;
        }
    }
}
