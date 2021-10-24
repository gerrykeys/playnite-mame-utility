using MAMEUtility.Models;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAMEUtility.Services.Cache
{
    class DataCache
    {
        public static Dictionary<string, MAMEMachine> mameMachines = new Dictionary<string, MAMEMachine>();

        //////////////////////////////////////////////////////
        public static MAMEMachine findMachineByPlayniteGame(Game playniteGame)
        {
            if (mameMachines.ContainsKey(playniteGame.Name)) 
                return mameMachines[playniteGame.Name];

            if (playniteGame.Roms.Count == 1 && mameMachines.ContainsKey(playniteGame.Roms[0].Name))
                return mameMachines[playniteGame.Roms[0].Name];

            return null;
        }
    }
}
