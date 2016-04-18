using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InTheHand.Net.Sockets;

namespace HeddokoLauncher.BluetoothSearch
{
    public static class BrainpackSearchResults
    {
        public static Dictionary<string, string> BrainpackToComPortMappings
        {
            get { return sBrainpackNameToComPort; }
        }
        private static Dictionary<string, string> sBrainpackNameToComPort = new Dictionary<string, string>(10);

        public static void AddComportDeviceCombo(BluetoothDeviceInfo vBtInfo, string vComport)
        {
            string vKey = vBtInfo.DeviceName;
            vKey= Regex.Replace(vKey, "(?i)adafruit(?-i)", "HEDDOKO");
            if (!sBrainpackNameToComPort.ContainsKey(vKey))
            {
                sBrainpackNameToComPort.Add(vKey, vComport);
            }
        }

        public static void ResetBrainpackSearchResults()
        {
            sBrainpackNameToComPort = new Dictionary<string, string>(10);
        }
        public static List<string> GetBluetoothDeviceNames()
        {
            return new List<string>(sBrainpackNameToComPort.Keys);
        }
    }

}
