using Pico.Conversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pico.Arrays
{
	public static class ArrayTools
    {
        #region Compare
        /// <summary>
        /// Compares two arrays, and returns if
        /// they are the same.
        /// </summary>
        /// <param name="arr1">First array.</param>
        /// <param name="arr2">Second array.</param>
        /// <returns></returns>
        public static bool Compare(byte[] arr1, byte[] arr2)
        {

            if (arr1 == null && arr2 == null) return true;  //Both null
            if (arr1 == null && arr2 != null) return false; //One null
            if (arr2 == null && arr1 != null) return false; //One null

            if (arr1.Length != arr2.Length) return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i]) return false;
            }

            return true;
        }
        #endregion

        #region Specific Index of Char in String
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
        #endregion

        #region Sort
        /// <summary>
        /// Sort an array.
        /// </summary>
        /// <param name="array">Array you want to sort.</param>
        /// <param name="highToLow">Low-to-high, or high-to-low.</param>
        /// <param name="valueDeterminant">Custom method for determining value.  Example: If you want to sort a Vector3[] by magnitude, method shall return vector3.Length()</param>
        /// <returns></returns>
        public static bool Sort<T>(T[] array, bool highToLow = false, Func<T, double> valueDeterminant = null)
        {
            if (array == null) return false;  //Not an array, silly.
            if (valueDeterminant == null) valueDeterminant = DefaultValueDeterminant;

            var values = AllElementValues(array, valueDeterminant);

            double tmpValue = 0;
            object tmpElement = null;

            bool done = false;
            while (true)
            {
                done = true;
                for (int i = 0; i < array.Length - 1; i++)
                {
                    //Compare current element to the next one
                    //Determine if should swap
                    //If a swap is made, flag 'not done' to repeat the process
                    switch (highToLow)
                    {
                        case false:
                            if (values[i + 1] < values[i]) Swap(i, i + 1);
                            break;
                        case true: //Largest to smallest
                            if (values[i + 1] > values[i]) Swap(i, i + 1);
                            break;
                    }
                }
                if (done) break;
            }
            return true;

            void Swap(int index1, int index2)
            {
                //Swap values and elements
                tmpElement = array.GetValue(index1);
                tmpValue = values[index1];

                array.SetValue(array.GetValue(index2), index1);
                values[index1] = values[index2];

                array.SetValue(tmpElement, index2);
                values[index2] = tmpValue;

                done = false;
            }
            double[] AllElementValues(Array array, Func<T, double> valueDeterminant)
            {
                double[] elementValues = new double[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    var element = array.GetValue(i);
                    elementValues[i] = valueDeterminant.Invoke((T)element);
                }
                return elementValues;
            }
            double DefaultValueDeterminant(T obj)
            {
                if (obj is IConvertible) return Convert.ToDouble(obj);
                return 0;
            }
        }
        #endregion

        #region Search
        /// <summary>
        /// Find first element within an array
        /// that matches search specs.
        /// </summary>
        /// <param name="array">Array to search through.</param>
        /// <param name="determinant">Function which defines what you're searching for.  Example, find first int less than 10: bool Determine(int num) => num < 10</param>
        /// <returns></returns>
        public static T Search<T>(T[] array, Func<T, bool> determinant)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var element = (T)array.GetValue(i);
                if (determinant.Invoke(element)) return element;
            }
            return default;
        }
        /// <summary>
        /// Find all elements within an array
        /// that match search specs.
        /// </summary>
        /// <param name="array">Array to search through.</param>
        /// <param name="determinant">Function which defines what you're searching for.  Example, find all ints less than 10: bool Determine(int num) => num < 10</param>
        /// <returns></returns>
        public static T[] SearchAll<T>(T[] array, Func<T, bool> determinant)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                var element = (T)array.GetValue(i);
                if (determinant.Invoke(element)) result.Add(element);
            }
            return result.ToArray();
        }
        #endregion

        #region Convert To String / Print
        public static string ToString<T>(T[,] array, int rows, int columns)
        {
            if (array == null) return "[null]";
            if (array.Length == 0) return "[empty]";
            StringBuilder sb = new StringBuilder();


            for (int r = 0; r < rows; r++)
            {
                sb.Append('[');
                for (int c = 0; c < columns; c++)
                {
                    sb.Append(array[r, c]);
                    if (c != columns - 1)
                    {
                        //Not last column
                        sb.Append(',');
                    }
                }
                sb.Append(']');
                if (r != rows - 1)
                {
                    //Not last row
                    sb.Append('\n');
                }
            }


            return sb.ToString();
        }
        public static string ToString<T>(T[] array)
        {
            if (array == null) return "[null]";
            if (array.Length == 0) return "[empty]";
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i]);
                if (i != array.Length - 1)
                {
                    //Not last column
                    sb.Append(',');
                }
            }
            sb.Append(']');
            return sb.ToString();
        }
        public static void Print<T>(T[,] array, int rows, int columns)
        {
            var text = ToString(array, rows, columns);
            Console.WriteLine(text);
        }
        public static void Print<T>(T[] array)
        {
            var text = ToString(array);
            Console.WriteLine(text);
        }
        #endregion



        #region PrintCompare
        /// <summary>
        /// Compares two int arrays, and highlights any differences.
        /// </summary>
        /// <param name="arr1">Array One.</param>
        /// <param name="arr2">Array Two.</param>
        public static void PrintCompare<T>(T[] arr1, T[] arr2)
        {
            if (arr1 == null && arr2 != null)
            {
                Console.WriteLine("[INVALID]");
                return;
            }
            if (arr2 == null && arr1 != null)
            {
                Console.WriteLine("[INVALID]");
                return;
            }
            int longest;
            if (arr1.Length > arr2.Length)
            {
                longest = arr1.Length;
            }
            else longest = arr2.Length;


            //Determine changed
            bool[] changed = new bool[longest];
            for (int i = 0; i < longest; i++)
            {
                if (i >= arr1.Length || i >= arr2.Length)
                {
                    changed[i] = true;
                    continue;
                }

                if (!arr1[i].Equals(arr2[i]))
                {
                    changed[i] = true;
                    continue;
                }
                changed[i] = false;
            }

            //Print Arr1
            WriteArray(arr1);


            //Print Arr2
            WriteArray(arr2);


            void WriteArray<T>(T[] array)
            {
                for (int i = 0; i < longest; i++)
                {
                    if (i < array.Length)
                    {
                        WriteString(array[i].ToString(), changed[i]);
                    }
                    else
                    {
                        WriteString(" ", changed[i]);
                    }

                    if (i != longest - 1) Console.Write(',');
                }
                Console.WriteLine();
            }



            void WriteString(string text, bool highlight)
            {
                if (!highlight)
                {
                    //Print
                    Console.Write(text);
                    return;
                }
                //Highlight
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                //Print
                Console.Write(text);
                //Reset
                Console.ResetColor();
            }

            #endregion
        }
    }
}