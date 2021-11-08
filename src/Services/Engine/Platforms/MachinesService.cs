using MAMEUtility.Models;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms.FBNeo;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAMEUtility.Services.Engine.Platforms
{
    //////////////////////////////////////////////////////////////
    public enum SourceType { MAMEExecutable, DATFile }

    //////////////////////////////////////////////////////////////
    public enum MachineType { Unknow, MAME, FBNeo }

    //////////////////////////////////////////////////////////////
    class MachinesResponseData
    {
        public MachinesResponseData() { }

        public Dictionary<string, RomsetMachine> machines;
        public bool isOperationCancelled;
        public bool isOperationError;
    }

    //////////////////////////////////////////////////////////////
    class MachinesService
    {
        //////////////////////////////////////////////////////////////
        public static MachineType getMachineTypeFromString(string type)
        {
            if (type == "MAME") return MachineType.MAME;
            if (type == "FBNeo") return MachineType.FBNeo;

            return MachineType.MAME;
        }

        //////////////////////////////////////////////////////////////
        public static SourceType getSourceType()
        {
            return (MAMEUtilityPlugin.settings.Settings.UseMameExecutable) ? SourceType.MAMEExecutable : SourceType.DATFile;
        }

        //////////////////////////////////////////////////////////////
        public static MachinesResponseData getMachines()
        {
            // get source type
            SourceType sourceType = getSourceType();

            MachinesResponseData responseData = new MachinesResponseData();
            MachineType machineType = MachineType.Unknow;

            // Case of no cached data
            if (Cache.DataCache.mameMachines.Count == 0)
            {
                // Case of MAME Executable
                if (sourceType == SourceType.MAMEExecutable)
                {
                    machineType = MachineType.MAME;
                }

                // Case of DAT
                else
                {
                    machineType =  getMachineTypeFromString(MAMEUtilityPlugin.settings.Settings.SelectedSourceFileListType);
                }

                // get machines
                responseData = getMachines(machineType);

                // store to cache
                if(responseData.machines != null)
                    Cache.DataCache.mameMachines = responseData.machines;

                return responseData;
            }

            // Case of cached data: Ask to user if wants to use cached data or regenerate MAME machines
            DialogResult dlgResult = UI.UIService.openAskDialog("", "Romset machines was previously generated. Do you want to use cached data? If no, then a rescan will be launched");
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

            // Regenerate machines
            responseData = getMachines(machineType);
            
            // store to cache
            if (responseData.machines != null)
                Cache.DataCache.mameMachines = responseData.machines;

            return responseData;
        }

        //////////////////////////////////
        private static MachinesResponseData getMachines(MachineType machineType)
        {
            MachinesResponseData responseData;

            // Get machines by machine type
            if (machineType == MachineType.MAME) responseData = MAMEMachinesService.getMachines();
            else if (machineType == MachineType.FBNeo) responseData = FBNeoMachinesService.getMachines();
            else return null;

            // Link parent/clones
            linkParentClones(responseData.machines);

            return responseData;
        }

        //////////////////////////////////////////////////////
        public static RomsetMachine findMachineByPlayniteGame(Dictionary<string, RomsetMachine> mameMachines, Game playniteGame)
        {
            if (mameMachines.ContainsKey(playniteGame.Name))
                return mameMachines[playniteGame.Name];

            if (playniteGame.Roms.Count == 1 && mameMachines.ContainsKey(playniteGame.Roms[0].Name))
                return mameMachines[playniteGame.Roms[0].Name];

            return null;
        }

        //////////////////////////////////////////////////////
        private static void linkParentClones(Dictionary<string, RomsetMachine> machinesMap)
        {
            foreach (KeyValuePair<string, RomsetMachine> entry in machinesMap)
            {
                RomsetMachine machine = entry.Value;
                if(machine.isClone())
                {
                    string parent = machine.cloneOf;
                    string clone  = entry.Key;
                    machinesMap[parent].clones.AddMissing<string>(clone);
                }
            }
        }
    }
}
