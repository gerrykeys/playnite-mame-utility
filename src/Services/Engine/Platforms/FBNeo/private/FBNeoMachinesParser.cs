using MAMEUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MAMEUtility.Services.Engine.Platforms.FBNeo
{
    class FBNeoMachinesParser
    {
        ////////////////////////////////////////////////////////////////
        public static Dictionary<string, RomsetMachine> parseMachines(XmlDocument gamelistXML)
        {
            Dictionary<string, RomsetMachine> mameGameMachines = new Dictionary<string, RomsetMachine>();

            XmlNodeList machineNodeList = gamelistXML.GetElementsByTagName("game");
            foreach (XmlNode machineNode in machineNodeList)
            {
                // Get MAME machine
                RomsetMachine mameMachine = parseMachine(machineNode);

                // Add machine to cache
                mameGameMachines.Add(mameMachine.romName, mameMachine);
            }

            return mameGameMachines;
        }

        ////////////////////////////////////////////////////////////////
        public static RomsetMachine parseMachine(XmlNode machineXmlNode)
        {
            if (machineXmlNode.Attributes == null) return null;

            RomsetMachine machine = new RomsetMachine();

            if (machineXmlNode.Attributes["name"] != null) machine.romName = machineXmlNode.Attributes["name"].Value;
            if (machineXmlNode.FirstChild != null) machine.description = machineXmlNode.FirstChild.InnerText;
            if (machineXmlNode.Attributes["cloneof"] != null) machine.cloneOf = machineXmlNode.Attributes["cloneof"].Value;
            if (machineXmlNode.Attributes["sampleof"] != null) machine.sampleof = machineXmlNode.Attributes["sampleof"].Value;
            if (machineXmlNode.Attributes["isbios"] != null) machine.isBios = getBoolFromYesNo(machineXmlNode.Attributes["isbios"].Value);
            if (machineXmlNode.Attributes["isdevice"] != null) machine.isDevice = getBoolFromYesNo(machineXmlNode.Attributes["isdevice"].Value);
            if (machineXmlNode.Attributes["ismechanical"] != null) machine.isMechanical = getBoolFromYesNo(machineXmlNode.Attributes["ismechanical"].Value);

            return machine;
        }

        ////////////////////////////////////////////////////////////
        private static bool getBoolFromYesNo(string str)
        {
            return (str == "yes") ? true : false;
        }
    }
}
