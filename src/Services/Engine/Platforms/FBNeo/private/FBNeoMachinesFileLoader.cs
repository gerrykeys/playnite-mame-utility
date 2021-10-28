using MAMEUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MAMEUtility.Services.Engine.Platforms.FBNeo
{
    class FBNeoMachinesFileLoader
    {
        ////////////////////////////////////////////
        public static Dictionary<string, RomsetMachine> getMachinesFromSourceListFile(string fileListPath)
        {
            try
            {
                // Load source list file
                XmlDocument machinesXmlList = new XmlDocument();
                machinesXmlList.Load(fileListPath);

                // Parse machines
                return FBNeoMachinesParser.parseMachines(machinesXmlList);

            }
            catch (System.Exception) { }

            return null;
        }
    }
}
