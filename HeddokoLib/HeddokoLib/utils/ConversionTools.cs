 
using System;

namespace HeddokoLib.utils
{
    public class ConversionTools
    {
        /// <summary>
        /// Converts the passed in Hex string and returns its float representation
        /// </summary>
        /// <param name="vHexVal">the string with hex values that needs to be converted</param>
        /// <returns>The float value of vHexVal</returns>
        public static float ConvertHexStringToFloat(string vHexVal)
        {
            //try swaping the bytes
            //string swapped = "0000";
            if (vHexVal.Length >= 4)
            {
                string byte1 = vHexVal[0] + vHexVal[1].ToString();
                string byte2 = vHexVal[2] + vHexVal[3].ToString();
                Byte byte_1 = Byte.Parse(byte1, System.Globalization.NumberStyles.HexNumber);
                Byte byte_2 = Byte.Parse(byte2, System.Globalization.NumberStyles.HexNumber);

                int data = byte_1 | (byte_2 << 8);
                float fVal = (float) (data << 16);
                fVal = fVal/(1 << 29);
                return fVal;
            }
            return 0.0f;
        }

    }
}
