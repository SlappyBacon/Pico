using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Pico.Conversion
{
    /// <summary>
    /// Abstract conversion tools.
    /// </summary>
    public static class ConvertTools
    {
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
        #region Byte(s) - Hex
        /// <summary>
        /// Converts each byte to one two-char hex word.
        /// (0 to 255) => (00 to FF)
        /// </summary>
        /// <param name="array">Bytes to convert.</param>
        /// <returns>Hex.</returns>
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
        /// <summary>
        /// Converts each two-char hex word to one byte.
        /// (00 to FF) => (0 to 255) 
        /// </summary>
        /// <param name="hex">Hex to convert.</param>
        /// <returns>Bytes.</returns>
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
        /// <summary>
        /// Converts one byte to one two-char hex word.
        /// (0 to 255) => (00 to FF)
        /// </summary>
        /// <param name="num">Byte to convert.</param>
        /// <returns>Hex.</returns>
        public static string ByteToHex(byte num)
        {
            string hexByte = num.ToString("X");
            if (hexByte.Length == 1) hexByte = "0" + hexByte;
            return hexByte;
        }
        /// <summary>
        /// Converts one two-char hex word to one byte.
        /// (00 to FF) => (0 to 255) 
        /// </summary>
        /// <param name="hex">Hex to convert.</param>
        /// <returns>Byte.</returns>
        public static byte HexToByte(string hex)
        {
            try
            {
                return Convert.ToByte(hex, 16);
            }
            catch
            {
                Console.WriteLine("fail");
                return 0;
            }
        }
        #endregion
        #region Notations
        /// <summary>
        /// Display a number count in it's readable form.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static string FormatBytes(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0) return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }


        /// <summary>
        /// Returns the given number in scientific notation.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string SciNo(float number, string unit = "U")
        {
            //This method should be re-written...
            //Perhapse more like FormatBytes() ?
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
