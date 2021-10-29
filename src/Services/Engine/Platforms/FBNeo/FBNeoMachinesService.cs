﻿using MAMEUtility.Models;
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
            GlobalProgressResult progressResult = UI.UIService.showProgress("Generating Romset data from FBNeo source file", false, true, (progressAction) => {
                mameMachines = FBNeoMachinesFileLoader.getMachinesFromSourceListFile(mameListFilePath);
            });

            return mameMachines;
        }
    }
}
