
using System.Collections.Generic;

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

        public List<string> clones = new List<string>();

        ////////////////////////////////////////////////////////////
        public bool isGame()
        {
            return (isBios || isDevice || isSample()) ? false : true;
        }

        ////////////////////////////////////////////////////////////
        public bool isParent()
        {
            return (string.IsNullOrEmpty(cloneOf)) ? true : false;
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
