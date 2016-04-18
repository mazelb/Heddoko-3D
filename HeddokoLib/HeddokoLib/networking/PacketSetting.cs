using System;
using System.Collections.Generic;
using System.Text;

namespace HeddokoLib.networking
{
    /// <summary>
    /// static settings for packets
    /// </summary>
    public static class PacketSetting
    {
        public static Encoding Encoding = Encoding.UTF8;
        public static char EndOfCommandDelim = '$';
        public static string EndOfPacketDelim = "<EOL>";
        public static int PacketCommandSize = 4;
    }
}
