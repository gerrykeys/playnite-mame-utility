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
        public static Dictionary<string, RomsetMachine> mameMachines = new Dictionary<string, RomsetMachine>();
    }
}
