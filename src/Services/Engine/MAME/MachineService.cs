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
    class MachineService
    {
        ////////////////////////////////////////////////////////////////////////////////
        public static Dictionary<string, MAMEMachine> getMachines(ref bool isOperationCanceled)
        {
            // Case of no cached data
            if(Cache.DataCache.mameMachines.Count == 0)
            {
                // Check MAME executable
                string mameExecutable = MAMEUtilityPlugin.settings.Settings.MameExecutableFilePath;
                if (string.IsNullOrEmpty(mameExecutable))
                {
                    MAMEUtilityPlugin.playniteAPI.Dialogs.ShowErrorMessage("Missing MAME executable", "You have to add MAME executable from extension settings");
                    return null;
                }

                // Get gamelist from MAME executable
                Dictionary<string, MAMEMachine> mameMachines = new Dictionary<string, MAMEMachine>();
                GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Gamelist data from MAME executable", false, true, (progressAction) => {
                    mameMachines = MAMECliExecutor.getMachinesFromMameExecutable(mameExecutable);
                });

                return mameMachines;
            }

            // Otherwise ask to use if want to use cached data or star a new machine rescan
            DialogResult dlgResult = UI.UIService.openAskDialog("", "Gamelist from MAME executable was previously generated. Do you want to use cached data? If no, then a rescan will be launched");
            if (dlgResult == DialogResult.Cancel) { isOperationCanceled = true;  return null; }
            if (dlgResult == DialogResult.Yes)    return Cache.DataCache.mameMachines;
            if (dlgResult == DialogResult.No)     return MAMECliExecutor.getMachinesFromMameExecutable(MAMEUtilityPlugin.settings.Settings.MameExecutableFilePath);
            
            return null;
        }
    }
}
