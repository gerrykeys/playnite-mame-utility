using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MAMEUtility.Models
{
    class RomsetMachine
    {
        public string romName;
        public string description;
        public string cloneOf;
        public string sampleof;
        public bool isBios;
        public bool isDevice;
        public bool isMechanical;

        ////////////////////////////////////////////////////////////
        public bool isGame()
        {
            return (isBios || isDevice || isSample()) ? false : true;
        }

        ////////////////////////////////////////////////////////////
        public bool isClone()
        {
            return (!string.IsNullOrEmpty(cloneOf)) ? true : false;
        }

        ////////////////////////////////////////////////////////////
        public bool isSample()
        {
            if (string.IsNullOrEmpty(sampleof)) return false;
            if (romName == sampleof)            return false;
            return true;
        }
    }
}
