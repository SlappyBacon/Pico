using System;
using System.Text;

namespace Pico.Randoms
{
	public static class RandomTools
    {
        #region Random String
        //Must provide a length
        //Optionally can provide a sring or char[] to sample characters from

        //These are the default characters, if no samples are provided
        static readonly char[] DefaultRandomStringChars = new char[]
        {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
                'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
                'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6',
                '7', '8', '9'
        };

        public static string RandomString(int length, string chars)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < length;)
            {
                text.Append(chars[Random.Shared.Next(0, chars.Length)]);
                i++;
            }
            return text.ToString();
        }
        public static string RandomString(int length, char[] chars = null)
        {
            if (chars == null) chars = DefaultRandomStringChars;
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < length;)
            {
                text.Append(chars[Random.Shared.Next(0, chars.Length)]);
                i++;
            }
            return text.ToString();
        }
        #endregion

        public static byte[] RandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++) bytes[i] = (byte)Random.Shared.Next(0, 256);
            return bytes;
        }

        public static double RandomDouble(double n1 = 1, double n2 = 0)
        {
            double min, max;
            if (n1 < n2)
            {
                min = n1;
                max = n2;
            }
            else if (n1 > n2)
            {
                min = n2;
                max = n1;
            }
            else
            {
                //numbers are the same
                return n1;
            }
            var span = max - min;
            var add = Random.Shared.NextDouble() * span;
            var result = min + add;
            return result;
        }
    }
}