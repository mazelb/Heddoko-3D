/** 
* @file BrainpackSerialPortSearch.cs
* @brief Contains the BrainpackSerialPortSearch class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using System;
using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;
using HeddokoLauncher.BluetoothSearch;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BrainpackService.BrainpackTools.BluetoothSearch
{
    /// <summary>
    /// Searches and sanitizes a list of available virtual comports
    /// </summary>
    static class BrainpackSerialPortSearch
    {
        /// <summary>
        /// The sanitized list of bluetooth virtual comports
        /// </summary>
        public static List<string> SanitizedList { get; private set; } = new List<string>();

        private static bool sCompletedSearch = false;
        private static int count = 0;
        static BluetoothDeviceInfo[] devices;
        private static bool sSearchCompleted
        {
            get
            {
                return sCompletedSearch;
            }
            set { sCompletedSearch = value; }
        }



        /// <summary>
        /// Searches for nearby devices and immediately calls the callback action on completion
        /// </summary> 
        public static List<string> Search()
        {

            //reset search results
            BrainpackSearchResults.ResetBrainpackSearchResults();
            // start searching for brainpacks in the immediate area
            if (BluetoothRadio.IsSupported)
            {
                //Look for devices in range
                BluetoothClient client = new BluetoothClient();
                devices = client.DiscoverDevicesInRange();
                ManagementObjectSearcher searcher = null;
                ManagementOperationObserver vResults = new ManagementOperationObserver();

                //Attach Handler events for results and completion
                vResults.ObjectReady += NewObject;
                vResults.Completed += (OnCompletion);

                string query = "SELECT  Name , DeviceID,PNPDeviceID FROM " +
                               "Win32_PnPEntity WHERE ClassGuid= \"{4d36e978-e325-11ce-bfc1-08002be10318}\"";

                searcher = new ManagementObjectSearcher(query);
                searcher.Get(vResults);

                while (!sSearchCompleted)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }

            sSearchCompleted = false;
            count = 0;
            return BrainpackSearchResults.GetBluetoothDeviceNames();
        }
        static void NewObject(object sender, ObjectReadyEventArgs obj)
        {

            //pattern for searching the word 
            //heddoko
            //case ignored
            string vSearchPattern = "(?i)heddoko(?-i)|(?i)adafruit(?-i)";


            foreach (BluetoothDeviceInfo d in devices)
            {

                if (Regex.IsMatch(d.DeviceName, vSearchPattern, RegexOptions.IgnoreCase))
                {
                    try
                    {

                        foreach (var mo in obj.NewObject.SystemProperties)
                        {
                            // mo.Origin 
                            try
                            {
                                string vPnpDeviceId = obj.NewObject["PNPDeviceID"].ToString();
                                if (Regex.IsMatch(vPnpDeviceId, d.DeviceAddress + "", RegexOptions.IgnoreCase))
                                {
                                    string vNameProperty = obj.NewObject["Name"].ToString();
                                    //strip com followed by numerical values
                                    int vIndex = vNameProperty.IndexOf("com", StringComparison.OrdinalIgnoreCase);
                                    if (vIndex > -1)
                                    {
                                        string vSubstring = "COM";
                                        //increment by 3
                                        vIndex += 3;
                                        while (vIndex < vNameProperty.Length)
                                        {
                                            char vValAt = vNameProperty[vIndex];
                                            if (Char.IsDigit(vValAt))
                                            {
                                                vSubstring += vValAt;
                                                vIndex++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        //validate string
                                        string vStrRegex = @"^(?i)COM(?-i)(\d+)?$";
                                        if (Regex.IsMatch(vSubstring, vStrRegex))
                                        {
                                            BrainpackSearchResults.AddComportDeviceCombo(d, vSubstring);

                                        }

                                    }


                                }
                            }
                            catch
                            {

                            }

                        }
                    }
                    catch (Exception)
                    {

                    }



                }
            }
        }

        static void OnCompletion(object sender,
         CompletedEventArgs obj)
        {
            sSearchCompleted = true;
        }


    }
}