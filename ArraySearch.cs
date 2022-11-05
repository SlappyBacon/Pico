using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Arrays
{
    public static class ArraySearch
    {
        #region Text Arrays
        public static int[] LinesThatStartWith(string find, in string[] lines, bool caseSensitive = true)
        {
            //Return the indeces of all lines which contain findText
            if (!caseSensitive) find = find.ToLower();  //Once
            List<int> result = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                string lineText = lines[i];
                if (!caseSensitive) lineText = lineText.ToLower();
                if (lineText.StartsWith(find)) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] LinesThatContain(string find, in string[] lines, bool caseSensitive = true)
        {
            //Return the indeces of all lines which contain findText
            if (!caseSensitive) find = find.ToLower();  //Once
            List<int> result = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                string lineText = lines[i];
                if (!caseSensitive) lineText = lineText.ToLower();
                if (lineText.Contains(find)) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] LinesThatEndWith(string find, in string[] lines, bool caseSensitive = true)
        {
            //Return the indeces of all lines which contain findText
            if (!caseSensitive) find = find.ToLower();  //Once
            List<int> result = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                string lineText = lines[i];
                if (!caseSensitive) lineText = lineText.ToLower();
                if (lineText.EndsWith(find)) result.Add(i);
            }
            return result.ToArray();
        }
        #endregion
        #region Int Arrays
        public static int[] NumsGreaterThan(int value, in int[] numbers)
        {
            //Return the indeces of all ints greater than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsLessThan(int value, in int[] numbers)
        {
            //Return the indeces of all ints less than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] < value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsEqualTo(int value, in int[] numbers)
        {
            //Return the indeces of all ints equal to the find value
            
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == value) result.Add(i);
            }
            return result.ToArray();
        }
        #endregion
        #region Long Arrays
        public static int[] NumsGreaterThan(long value, in long[] numbers)
        {
            //Return the indeces of all ints greater than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsLessThan(long value, in long[] numbers)
        {
            //Return the indeces of all ints less than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] < value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsEqualTo(long value, in long[] numbers)
        {
            //Return the indeces of all ints equal to the find value

            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == value) result.Add(i);
            }
            return result.ToArray();
        }
        #endregion
        #region Float Arrays
        public static int[] NumsGreaterThan(float value, in float[] numbers)
        {
            //Return the indeces of all ints greater than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsLessThan(float value, in float[] numbers)
        {
            //Return the indeces of all ints less than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] < value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsEqualTo(float value, in float[] numbers)
        {
            //Return the indeces of all ints equal to the find value

            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == value) result.Add(i);
            }
            return result.ToArray();
        }
        #endregion
        #region Double Arrays
        public static int[] NumsGreaterThan(double value, in double[] numbers)
        {
            //Return the indeces of all ints greater than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsLessThan(double value, in double[] numbers)
        {
            //Return the indeces of all ints less than the find value
            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] < value) result.Add(i);
            }
            return result.ToArray();
        }
        public static int[] NumsEqualTo(double value, in double[] numbers)
        {
            //Return the indeces of all ints equal to the find value

            List<int> result = new List<int>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == value) result.Add(i);
            }
            return result.ToArray();
        }
        #endregion
    }
}
