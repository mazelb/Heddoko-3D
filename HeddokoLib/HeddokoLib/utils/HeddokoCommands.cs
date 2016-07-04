 
using System.Text;
using HeddokoLib.networking;

namespace HeddokoLib.utils
{
    /// <summary>
    /// Commands to be used by a command router
    /// </summary>
    public static class HeddokoCommands
    {
        public static string RequestServerConnection = "0x40";
        public static string RequestBrainpackList = "0x00";
        public static string SendBrainpackList = "0x01";
        public static string RequestToConnectToBP = "0x02"; 
        public static string BPConnectionFailure = "0x03";
        public static string BPConnectionSucess = "0x04";
        public static string BPDeviceLost = "0x05";
        public static string CloseConnectionToServer = "0x06";
        public static string SendBPData = "0x07";
        public static string SendBTCommand = "0x08";
        public static string RequestBPData = "0x09";
        public static string RequestConnection = "0x10";
        public static string ConnectionAck = "0x11";
        public static string UseRecordingForBody = "0x12";
        public static string UseBrainpackForBody = "0x13";
        public static string StartSceneWithBrainpackConnect = "0x14";
        public static string DisconnectBrainpack = "0x15";
        public static string DiscoAcknowledged = "0x16";

        #region brainpack changes request
        public static string SetBrainpackTimeReq = "0x17";
        public static string SetRecordingPrefixReq = "0x18";
        public static string ShutdownBrainpackReq = "0x19";
        public static string ResetBrainpackReq = "0x20";
        public static string GetBrainpackVersionReq = "0x21";
        public static string GetBrainpackStateReq = "0x22";
        #endregion

        #region  brainpack changes responses
        public static string SetBrainpackTimeResp = "0x23";
        public static string SetRecordingPrefixResp = "0x24";
        public static string ShutdownBrainpackResp = "0x25";
        public static string ResetBrainpackResp = "0x26";
        public static string GetBrainpackVersionResp = "0x26";
        public static string GetBrainpackStateResp = "0x27";
        #endregion
        public static string StartRecordingReq = "0x28";
        public static string StartRecordingResp = "0x29";
        public static string StopHeddokoUnityClient = "9x99";

        public static string GenericAckMessage = "0x30";
        public static string MessagePacket= "0x31";
        public static string GetResponseMessageReq = "0x32";
        public static string GetResponseMessageResp = "0x33";
        public static string StopRecordingReq = "0x34";
        public static string StopRecordingResp = "0x35";
        public static string ClearBuffer = "0x36";
        public static string DisableSleepTimerReq = "0x37";
        public static string DisableSleepTimerResp = "0x38";
        public static string EnableSleepTimerReq = "0x39";
        public static string EnableSleepTimerResp = "0x42";
        public static string RegisterListener = "0x43";
        public static string RegisterListenerAck = "0x44";
        public static string GetBrainpackResults = "0x45";
        public static string ReturnBrainpackResults = "0x46";
        public static string SendCommand = "0x47";
        public static string ClientError = "9x00";
     

        public static string ExtractCommandFromBytes(int vStartIndex, int vLength, byte[] vData)
        {
            string vCommand = "";
            byte[] vExtractedCommand = new byte[vLength];
            int vIteratingIndex = 0;
            for (int i = vStartIndex; i < vLength; i++, vIteratingIndex++)
            {
                vExtractedCommand[vIteratingIndex] = vData[i];
            }
            StringBuilder vSb = new StringBuilder();
            vSb.Append(PacketSetting.Encoding.GetString(vExtractedCommand, 0, vLength));

            vCommand = vSb.ToString();
            return vCommand;
        }

    }
}
