using System;
using BrainpackService.Tools_and_Utilities;

namespace BrainpackService.brainpack_serial_connect
{
    class ImuEntry
    {
        private double yaw;
        private double pitch;
        private double roll;

        public ImuEntry(string rawEntry)
        {
            //char separator[] = ';';
            string[] separatedEntry = rawEntry.Split(';');
            if (separatedEntry.Length < 3)
            {
                //this is an error
                BrainpackEventLogManager.InvokeEventLogMessage("error failed to parse data");
                this.yaw = 0.0;
                this.pitch = 0.0;
                this.roll = 0.0;
                return;
            }
            this.yaw = convertRawDataToFloat(separatedEntry[0]);
            this.pitch = convertRawDataToFloat(separatedEntry[1]);
            this.roll = convertRawDataToFloat(separatedEntry[2]);
        }
        public ImuEntry(double y, double p, double r)
        {
            this.yaw = y;
            this.pitch = p;
            this.roll = r;
        }

        public double Yaw
        {
            get
            {
                return yaw;
            }

            set
            {
                yaw = value;
            }
        }

        public double Pitch
        {
            get
            {
                return pitch;
            }

            set
            {
                pitch = value;
            }
        }

        public double Roll
        {
            get
            {
                return roll;
            }

            set
            {
                roll = value;
            }
        }

        override public string ToString()
        {
            return this.yaw.ToString("F6") + ";" + this.pitch.ToString("F6") + ";" + this.roll.ToString("F6");
        }


        static public double convertRawDataToFloat(string val)
        {
            //try swaping the bytes
            string swapped = "0000";
            if (val.Length >= 4)
            {
                swapped = val[2].ToString() + val[3].ToString() + val[0].ToString() + val[1].ToString();
                //swapped = val[0].ToString() + val[1].ToString() + val[2].ToString() + val[3].ToString();
                Int16 data = Int16.Parse(swapped, System.Globalization.NumberStyles.HexNumber);
                float fVal = (float)(data << 16);
                fVal = fVal / (1 << 29);

                return (double)fVal;
            }
            return 0.0;
        }

    }
}
