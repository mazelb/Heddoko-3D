 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;

namespace HeddokoLauncher.BluetoothSearch
{
    public delegate void SearchCompletion(Dictionary<string, string> vNameToComportMaps );
    public static class BrainpackSearchResults
    {
        public static Action<Dictionary<string, string>> SearchCompletionEventHandler;
        public static void StartSearch()
        {
            Task<List<string>> vTaskSearch = Task<List<string>>.Factory.StartNew(BTSearch.Search);
            vTaskSearch.ContinueWith(OnSearchCompletion);
        }

        private static void OnSearchCompletion(Task<List<string>> vObj)
        {
            SearchCompletionEventHandler?.Invoke(sBrainpackNameToComPort);
        }

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

        /// <summary>
        /// Reset search results of the brainpack
        /// </summary>
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
