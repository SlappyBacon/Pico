using Pico.Maths;
using System;
using System.Numerics;
using System.Text;

namespace Pico.Randoms
{
	public static class RandomTools
    {
        public static bool Chance(float percent) => Random.Shared.NextDouble() < percent;
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

        
        /// <summary>
        /// Returns a random string with given parameters.
        /// </summary>
        /// <param name="length">Length of string.</param>
        /// <param name="chars">Chars to sample from.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Returns a random string with given parameters.
        /// </summary>
        /// <param name="length">Length of string.</param>
        /// <param name="chars">Chars to sample from.</param>
        /// <returns></returns>
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
        #endregion
        #region Random Byte Array
        /// <summary>
        /// Returns a random byte[]
        /// </summary>
        /// <param name="length">Length of array.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value, inclusive.</param>
        /// <returns></returns>
        public static byte[] RandomByteArray(int length, byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            byte[] bytes = new byte[length];
            //Full range
            if (min == byte.MinValue && max == byte.MaxValue)
            {
                Random.Shared.NextBytes(bytes);
                return bytes;
            }
            //Specific range
            if (max < min) max = min;
            for (int i = 0; i < length; i++) bytes[i] = (byte)Random.Shared.Next(min, max + 1);
            return bytes;
        }
        #endregion
        #region Random Int Array
        /// <summary>
        /// Returns a random array.
        /// </summary>
        /// <param name="length">Length of array.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value, exclusive.</param>
        /// <returns></returns>
        public static int[] RandomIntArray(int length, int min = int.MinValue, int max = int.MaxValue)
        {
            int[] nums = new int[length];
            if (max < min) max = min;
            for (int i = 0; i < length; i++) nums[i] = Random.Shared.Next(min, max);
            return nums;
        }
        #endregion
        #region Random Double
        /// <summary>
        /// Returns a random double between two numbers.
        /// </summary>
        /// <param name="n1">First number.</param>
        /// <param name="n2">Second number.</param>
        /// <returns></returns>
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
        #endregion
        #region Random Vector3
        /// <summary>
        /// Returns a random Vector3 with a magnitude of 1.
        /// </summary>
        /// <returns></returns>
        public static Vector3 RandomVector3()
        {
            return MathTools.FibVector3(new Random().Next(0, int.MaxValue), int.MaxValue);
        }
        #endregion
        #region Random Ip Address
        public static string RandomIpAddress()
        {
            //xxx.xxx.xxx.xxx
            var n1 = Random.Shared.Next(0, 1000);
            var n2 = Random.Shared.Next(0, 1000);
            var n3 = Random.Shared.Next(0, 1000);
            var n4 = Random.Shared.Next(0, 1000);
            return $"{n1}.{n2}.{n3}.{n4}";
        }
        #endregion
    }
}