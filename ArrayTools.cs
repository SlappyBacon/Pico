using System;
using System.Linq;

namespace Pico.Arrays
{
    /// <summary>
    /// Very early version, not much exists yet.
    /// </summary>
	public static class ArrayTools
    {
        /// <summary>
        /// Compares two arrays, and returns if
        /// they are the same.
        /// </summary>
        /// <param name="arr1">First array.</param>
        /// <param name="arr2">Second array.</param>
        /// <returns></returns>
        public static bool ArraysAreSame(byte[] arr1, byte[] arr2)  //Add more types?
        {
            if (arr1 == null && arr2 == null) return true;
            if (arr1 == null && arr2 != null) return false;
            if (arr1 != null && arr2 == null) return false;
            if (arr1.Length != arr2.Length) return false;
            for (int i = 0; i < arr1.Length; i++) if (arr1[i] != arr2[i]) return false;
            return true;
        }

        /// <summary>
        /// Finds the index of a specific character.
        /// 
        /// Eg: The second 'o' in "Hello World" would
        ///     be character 7.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="character"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int SpecificIndexOf(string text, char character, int number = 1)
        {
            if (text == null || number < 1) return -1;
            int last = 0;
            for (int i = 0; i < number;)
            {
                int index = text.IndexOf(character, last + 1);
                if (index != -1) last = index;
                else return -1;
                i++;
            }
            return last;
        }
    }
}