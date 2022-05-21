using System;
using System.Numerics;
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
        #endregion



        /// <summary>
        /// Returns a random byte[]
        /// </summary>
        /// <param name="length">Length of byte[]</param>
        /// <returns></returns>
        public static byte[] RandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++) bytes[i] = (byte)Random.Shared.Next(0, 256);
            return bytes;
        }

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

        /// <summary>
        /// Returns a random Vector3 with a magnitude of 1.
        /// </summary>
        /// <returns></returns>
        public static Vector3 RandomVector3()
        {
            return FibVector3(new Random().Next(0, int.MaxValue), int.MaxValue);
        }


        /// <summary>
        /// Returns the Fibonacci Vector3, according
        /// to the given paramaters.
        /// </summary>
        /// <returns></returns>
        public static Vector3 FibVector3(int point, int points)
        {
            var k = point + .5f;

            var phi = Math.Acos(1f - 2f * k / points);
            var theta = Math.PI * (1 + Math.Sqrt(5)) * k;

            var x = Math.Cos(theta) * Math.Sin(phi);
            var y = Math.Sin(theta) * Math.Sin(phi);
            var z = Math.Cos(phi);

            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}