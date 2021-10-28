using MAMEUtility.Models;
using MAMEUtility.Services.Engine.MAME;
using MAMEUtility.Services.Engine.Platforms.FBNeo;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAMEUtility.Services.Engine.Platforms
{
    //////////////////////////////////////////////////////////////
    public enum MachineTypes { MAME, FBNeo }

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
        public static MachineTypes getMachineTypeFromString(string type)
        {
            if (type == "MAME") return MachineTypes.MAME;
            if (type == "FBNeo") return MachineTypes.FBNeo;

            return MachineTypes.MAME;
        }

        //////////////////////////////////
        public static MachinesResponseData getMachines(MachineTypes machineType)
        {
            if(machineType == MachineTypes.MAME)  return MAMEMachinesService.getMachines();
            if (machineType == MachineTypes.FBNeo) return FBNeoMachinesService.getMachines();

            return null;
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
    }
}
