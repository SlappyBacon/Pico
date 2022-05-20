using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Pico.Conversion
{
    public static class ConvertTools
    {

        //OUTDATED\\
        //There's probs better ways to do some of these

        #region Time - String
        /// <summary>
        /// Converts System.DateTime to a string and keeps the milliseconds.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string TimeToString(DateTime time)
        {
            var timeString = time.ToString();
            int amOrPmSplit = timeString.Length - 3;
            StringBuilder s = new StringBuilder();
            s.Append(timeString.Substring(0, amOrPmSplit));//before AM/PM;
            s.Append(":");
            s.Append(time.Millisecond);
            string amOrPm = timeString.Substring(amOrPmSplit); //AM/PM
            s.Append(amOrPm);
            return s.ToString();
        }
        /// <summary>
        /// Converts string to a System.DateTime and keeps the milliseconds.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime StringToTime(string time)
        {
            //parse time without milliseconds
            int amOrPmSplit = time.Length - 3;
            string amOrPm = time.Substring(amOrPmSplit);
            int timeSplit = time.LastIndexOf(":");
            string parseTime = time.Substring(0, timeSplit);
            DateTime result = DateTime.Parse(parseTime + amOrPm);

            //add milliseconds
            int msStart = timeSplit + 1;
            string msString = time.Substring(msStart, time.IndexOf(" ", timeSplit) - msStart);
            int ms = int.Parse(msString);
            result = result + TimeSpan.FromMilliseconds(ms);
            return result;
        }
        #endregion
        #region String - Binary String
        /// <summary>
        /// Splits a string into characters, then constructs a binary string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>

        public static string StringToBinary(string text)
        {
            string result = "";
            foreach (char c in text.ToCharArray())
            {
                string binaryChar = Convert.ToString(c, 2);
                string zeros = "";
                for (int i = binaryChar.Length; i < 8;)
                {
                    zeros += "0";
                    i++;
                }
                //Add Zeros
                result += zeros + binaryChar;
            }
            return result;
        }
        /// <summary>
        /// Splits a binary string into binary characters, then constructs a normal string.
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static string BinaryToString(string binary)
        {
            string result = "";
            for (int i = 0; i < binary.Length;)   //Process Each Bit
            {
                int bits;
                if (binary.Length < 8) bits = binary.Length;
                else bits = 8;
                string character = binary.Substring(i, bits);
                result += (char)Convert.ToInt32(character, 2);
                i += bits;
            }
            return result;
        }
        #endregion
        #region String - Bytes
        public static byte[] StringToBytes(string text)
        {
            byte[] bytes = new byte[text.Length];
            for (int i = 0; i < bytes.Length;)
            {
                bytes[i] = (byte)text[i];
                i++;
            }
            return bytes;
        }
        public static string BytesToString(byte[] bytes)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < bytes.Length;)
            {
                text.Append((char)bytes[i]);
                i++;
            }
            return text.ToString();
        }
        #endregion
        #region Byte(s) - Hex
        //EVERY BYTE CONVERTS TO A 2-CHAR STRING
        //RANGE : 00 to FF  (0 to 255)
        public static string ByteArrayToHex(byte[] array)
        {
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < array.Length;)
            {
                string hexByte = ByteToHex(array[i]);
                hex.Append(hexByte);
                i++;
            }
            return hex.ToString();
        }
        public static byte[] HexToByteArray(string hex)
        {
            if (hex.Length % 2 != 0) return new byte[0];
            byte[] array = new byte[hex.Length/2];
            for (int i = 0; i < array.Length;)
            {
                string c = hex.Substring(i * 2, 2);
                var b = HexToByte(c);
                array[i] = b;
                i += 2;
            }
            return array;
        }
        public static string ByteToHex(byte num)
        {
            Console.Write(num + " => ");
            string hexByte = num.ToString("X");
            if (hexByte.Length == 1) hexByte = "0" + hexByte;
            Console.WriteLine(hexByte);
            return hexByte;
        }
        public static byte HexToByte(string hex)
        {
            Console.Write(hex + " => ");
            try
            {
                var b = Convert.ToByte(hex, 16);
                Console.WriteLine(b);
                return b;
            }
            catch
            {
                Console.WriteLine("fail");
                return 0;
            }
        }
        #endregion
        #region Notations
        public static string FormatBytes(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0) return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static string SciNo(float number, string unit = "U")
        {
            string note;
            if (number == 0)
            {
                note = "";
            }
            else if (number >= 1 && number < 1000)
            {
                note = "";
            }
            else if (number >= 0.001F && number < 1F)
            {
                number *= 1000F;
                note = "m";
            }
            else if (number >= 1000 && number < 1000000)
            {
                number /= 1000F;
                note = "k";
            }
            else if (number >= 0.000001F && number < 0.001F)
            {
                number *= 1000000F;
                note = "u";
            }
            else if (number >= 1000000 && number < 1000000000)
            {
                number /= 1000000F;
                note = "M";
            }
            else if (number >= 0.000000001F && number < 0.000001F)
            {
                number *= 1000000000F;
                note = "n";
            }

            else if (number >= 1000000000 && number < 1000000000000)
            {
                number /= 1000000000F;
                note = "G";
            }
            else if (number >= 0.000000000001F && number < 0.000000001F)
            {
                number *= 1000000000000F;
                note = "p";
            }

            else if (number >= 1000000000000 && number < 1000000000000000)
            {
                number /= 1000000000000F;
                note = "T";
            }
            else if (number >= 0.000000000000001F && number < 0.000000000001F)
            {
                number *= 1000000000000000F;
                note = "f";
            }

            else if (number >= 1000000000000000 && number < 1000000000000000000)
            {
                number /= 1000000000000000F;
                note = "P";
            }
            else if (number >= 0.000000000000000001F && number < 0.000000000000001F)
            {
                number *= 1000000000000000000F;
                note = "a";
            }

            else if (number >= 1000000000000000000 && number < 1000000000000000000000F)
            {
                number /= 1000000000000000000F;
                note = "E";
            }
            else if (number >= 0.000000000000000000001F && number < 0.000000000000000001F)
            {
                number *= 1000000000000000000000F;
                note = "z";
            }

            else if (number >= 1000000000000000000000F && number < 1000000000000000000000000F)
            {
                number /= 1000000000000000000000F;
                note = "Z";
            }
            else if (number >= 0.000000000000000000000001F && number < 0.000000000000000000001F)
            {
                number *= 1000000000000000000000000F;
                note = "y";
            }
            else if (number >= 1000000000000000000000000F && number < 1000000000000000000000000000F)
            {
                number /= 1000000000000000000000000F;
                note = "Y";
            }
            else
            {
                return null;    //Out of range
            }
            string text = number.ToString() + note + unit;
            return text;
        }
        #endregion
    }


}
