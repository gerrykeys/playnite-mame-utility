

using MAMEUtility.Models;
using Playnite.SDK;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace MAMEUtility.Services.Engine
{
    class MAMECliExecutor
    {
        ////////////////////////////////////////////////////////////////////////
        public static Dictionary<string, MAMEMachine> getMachinesFromMameExecutable(string mameExecutable)
        {
            try
            {
                // Generate MAME gamelist XML
                XmlDocument mameGamelistXML = generateGamelistXML(mameExecutable);

                return MachineParser.parseMachines(mameGamelistXML);
            
            }
            catch (System.Exception){}

            return null;
        }

        ////////////////////////////////////////////////////////////////////////
        private static XmlDocument generateGamelistXML(string mameExecutable)
        {
            try
            {
                // Call MAME executable to obtain gamelist
                ProcessStartInfo pStartInfo = new ProcessStartInfo();
                pStartInfo.FileName = mameExecutable;
                pStartInfo.Arguments = "-listxml";
                pStartInfo.UseShellExecute = false;
                pStartInfo.RedirectStandardOutput = true;
                pStartInfo.CreateNoWindow = true;
                Process process = new Process();
                process.StartInfo = pStartInfo;
                process.Start();
                string outputBuffer = process.StandardOutput.ReadToEnd();

                // Create xml object from buffer
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(outputBuffer);
                return doc;
            }
            catch (System.Exception){}
            return null;
        }
    }
}
