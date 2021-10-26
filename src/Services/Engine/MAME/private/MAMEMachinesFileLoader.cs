﻿using MAMEUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MAMEUtility.Services.Engine.MAME
{
    class MAMEMachinesFileLoader
    {
        ////////////////////////////////////////////
        public static Dictionary<string, MAMEMachine> getMachinesFromListFile(string mameFileListPath)
        {
            try
            {
                // Generate MAME gamelist XML
                XmlDocument mameMachineslistXML = new XmlDocument();
                mameMachineslistXML.Load(mameFileListPath);

                // Parse machines
                return MachineParser.parseMachines(mameMachineslistXML);

            }
            catch (System.Exception) { }

            return null;
        }
    }
}
