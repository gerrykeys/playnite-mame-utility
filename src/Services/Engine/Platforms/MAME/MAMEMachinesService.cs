using MAMEUtility.Models;
using MAMEUtility.Services.Engine.Platforms;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine.MAME
{
    class MAMEMachinesService
    {
        ////////////////////////////////////////////////////////////////////////////////
        public static MachinesResponseData getMachines()
        {
            MachinesResponseData responseData = new MachinesResponseData();

            // Case of no cached data
            if (Cache.DataCache.mameMachines.Count == 0)
            {
                // Generate MAME machines
                responseData.machines = generateMachines();
                return responseData;
            }

            // Case of cached data: Ask to user if wants to use cached data or regenerate MAME machines
            DialogResult dlgResult = UI.UIService.openAskDialog("", "MAME machines was previously generated. Do you want to use cached data? If no, then a rescan will be launched");
            if (dlgResult == DialogResult.Cancel) {
                responseData.isOperationCancelled = true;
                return responseData;
            }
            if (dlgResult == DialogResult.Yes) {
               responseData.machines = Cache.DataCache.mameMachines;
                return responseData;
            }

            responseData.machines = generateMachines();
            return responseData;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, RomsetMachine> generateMachines()
        {
            try
            {
                var settings = MAMEUtilityPlugin.settings.Settings;

                // generate mame machines
                Dictionary<string, RomsetMachine> mameMachines = (settings.UseMameExecutable) ? getMachinesFromMameExecutable(settings.MameExecutableFilePath) : getMachinesFromMameListFile(settings.SourceListFilePath);

                // update cache
                Cache.DataCache.mameMachines = mameMachines;

                return mameMachines;
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, RomsetMachine> getMachinesFromMameExecutable(string mameExecutablePath)
        {
            if (string.IsNullOrEmpty(mameExecutablePath))
            {
                MAMEUtilityPlugin.playniteAPI.Dialogs.ShowErrorMessage("Missing MAME executable", "You are using MAME executable as source but you have not set the MAME executable path from extension settings");
                return null;
            }

            // Get gamelist from MAME executable
            Dictionary<string, RomsetMachine> mameMachines = new Dictionary<string, RomsetMachine>();
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Gamelist data from MAME executable", false, true, (progressAction) => {
                mameMachines = MAMECliExecutor.getMachinesFromMameExecutable(mameExecutablePath);
            });

            return mameMachines;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, RomsetMachine> getMachinesFromMameListFile(string mameListFilePath)
        {
            if (string.IsNullOrEmpty(mameListFilePath))
            {
                UI.UIService.showError("Missing MAME source list file", "You are using MAME list file as source but you have not set the file path from extension settings");
                return null;
            }

            // Get gamelist from MAME executable
            Dictionary<string, RomsetMachine> mameMachines = new Dictionary<string, RomsetMachine>();
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Gamelist data from MAME executable", false, true, (progressAction) => {
                mameMachines = MAMEMachinesFileLoader.getMachinesFromListFile(mameListFilePath);
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
