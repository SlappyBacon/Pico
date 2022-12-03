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
        public static bool AreSame(Array arr1, Array arr2)
        {
            if (arr1 == null && arr2 == null) return true;      //Both null, same
            if (arr1 == null && arr2 != null) return false;     //Different types
            if (arr1 != null && arr2 == null) return false;     //Different types
            if (arr1.GetType() != arr2.GetType()) return false; //Different types
            if (arr1.Length != arr2.Length) return false;       //Different lengths                                              
            for (int i = 0; i < arr1.Length; i++)               //Different elements?
            {
                if (arr1.GetValue(i) != arr2.GetValue(i)) return false; 
            }
            return true;//Same
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
        /// <param name="order">Low-to-high, or high-to-low.</param>
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
        /// Find elements within an array.
        /// </summary>
        /// <param name="array">Array to search through.</param>
        /// <param name="determinant">Function which defines what you're searching for.  Example, find all ints less than 10: bool Determine(int num) => num < 10</param>
        /// <returns></returns>
        public static T[] Search<T>(T[] array, Func<T, bool> determinant)
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
        public static string ToString(object array, bool vertical = false)
        {
            if (array == null) return "[null]";
            Array arr = array as Array;
            if (arr.Length == 0) return "[empty]";
            StringBuilder sb = new StringBuilder();
            if (!vertical) sb.Append('[');
            for (int i = 0; i < arr.Length; i++)
            {
                var element = arr.GetValue(i);
                if (vertical) sb.Append($"{element},\n");
                else sb.Append($"{element},");
            }
            if (sb.Length > 0)
            {
                //Trim
                if (vertical) sb.Remove(sb.Length - 2, 2); //",\n"
                else sb.Remove(sb.Length - 1, 1); //","
            }
            if (!vertical) sb.Append(']');
            return sb.ToString();
        }
        public static void Print(object array, bool vertical = false) => Console.WriteLine(ToString(array, vertical));
        #endregion
    }
}